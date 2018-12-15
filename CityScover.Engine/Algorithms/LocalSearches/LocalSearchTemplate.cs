
//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 15/12/2018
//

using CityScover.Commons;
using CityScover.Engine.Algorithms.CustomAlgorithms;
using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Algorithms.VariableDepthSearch;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MoreLinq;

namespace CityScover.Engine.Algorithms.LocalSearches
{
   internal class LocalSearchTemplate : Algorithm
   {
      #region Private fields
      private readonly NeighborhoodFacade<ToSolution> _neighborhoodFacade;
      private int _previousSolutionCost;
      private int _iterationsWithoutImprovement;
      private bool _canDoImprovements;
      private bool _shouldRunImprovement;
      private int _improvementThreshold;
      private int _maxIterationsWithoutImprovements;
      private ICollection<ToSolution> _solutionsHistory;
      #endregion

      #region Constructors
      internal LocalSearchTemplate(Neighborhood<ToSolution> neighborhood, 
         AlgorithmTracker provider = null) : base(provider)
      {
         Type = neighborhood.Type;
         _neighborhoodFacade = new NeighborhoodFacade<ToSolution>(neighborhood);
      }
      #endregion

      #region Internal properties
      internal ToSolution CurrentBestSolution { get; set; }
      public int ImprovementsCount { get; private set; }
      #endregion

      #region Private methods
      private ToSolution GetBest(IEnumerable<ToSolution> neighborhood) => 
         neighborhood.MaxBy(solution => solution.Cost);

      private async Task RunImprovementsInternal()
      {
         var childrenFlow = Solver.CurrentStage.Flow.ChildrenFlows;

         foreach (var algorithm in Solver.GetImprovementAlgorithms(childrenFlow))
         {
            algorithm.Provider = Provider;

            var solutionToImprove = (CurrentBestSolution.Cost < Solver.BestSolution.Cost)
               ? Solver.BestSolution 
               : CurrentBestSolution;

            await RunImprovement(algorithm, solutionToImprove, Type);
            ImprovementsCount++;

            /* 
             * TODO
             * Verificare se necessario eseguire i successivi algoritmi dello StageFlow presenti tra i ChildrenFlows.
             * Usare un if-break per questo scopo sotto questo commento, dopo la terminazione dell'algoritmo.
             * Senza un eventuale controllo, verrebbero eseguiti in sequenza tutti gli algoritmi presenti tra i ChildrenFlows a prescindere, 
             * anche se non fosse necessario eseguirli.
             */
         }
      }
      #endregion

      #region Internal methods
      internal async Task RunImprovement(Algorithm algorithm, ToSolution startingSolution, AlgorithmType typeToRestart)
      {
         Algorithm improvementAlgorithm = default;

         switch (algorithm)
         {
            case HybridCustomUpdate hcu:
               improvementAlgorithm = hcu;
               hcu.CurrentBestSolution = startingSolution;
               break;

            case HybridCustomInsertion hci:
               improvementAlgorithm = hci;
               hci.CurrentBestSolution = startingSolution;
               break;

            case LinKernighan lk:
               improvementAlgorithm = lk;
               lk.CurrentBestSolution = startingSolution;
               break;
         }

         await StartImprovementAlgorithm(improvementAlgorithm, typeToRestart);
      }

      internal async Task StartImprovementAlgorithm(Algorithm algorithm, AlgorithmType typeToRestart)
      {
         Solver.CurrentAlgorithm = algorithm.Type;
         Task algorithmTask = Task.Run(algorithm.Start);

         try
         {
            await algorithmTask.ConfigureAwait(false);
         }
         catch (AggregateException ae)
         {
            OnError(ae.InnerException);
         }
         finally
         {
            Solver.CurrentAlgorithm = typeToRestart;
         }
      }

      internal void ResetState()
      {
         _previousSolutionCost = default;
         _iterationsWithoutImprovement = default;
         _shouldRunImprovement = default;
         _improvementThreshold = default;
         _maxIterationsWithoutImprovements = default;
         CurrentBestSolution = default;
         _solutionsHistory.Clear();
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _solutionsHistory = new Collection<ToSolution>();
         Solver.PreviousStageSolutionCost = Solver.BestSolution.Cost;
         _canDoImprovements = Parameters[ParameterCodes.CanDoImprovements];

         if (_canDoImprovements)
         {
            _improvementThreshold = Parameters[ParameterCodes.LSimprovementThreshold];
            _maxIterationsWithoutImprovements = Parameters[ParameterCodes.LSmaxRunsWithNoImprovements];
         }

         if (CurrentBestSolution is null)
         {
            CurrentBestSolution = Solver.BestSolution;
         }
         _solutionsHistory.Add(CurrentBestSolution);
         SendMessage(MessageCode.LocalSearchStartSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);
         ImprovementsCount = default;
         _previousSolutionCost = default;
         _shouldRunImprovement = default;
         _iterationsWithoutImprovement = default;
      }

      protected override async Task PerformStep()
      {
         SendMessage(MessageCode.LocalSearchNewNeighborhood, CurrentStep);

         var currentNeighborhood = _neighborhoodFacade.GenerateNeighborhood(CurrentBestSolution);

         foreach (var neighborSolution in currentNeighborhood)
         {
            SendMessage(MessageCode.LocalSearchNewNeighborhoodMove, neighborSolution.Id, CurrentStep);
            SendMessage(neighborSolution.Description);
            Solver.EnqueueSolution(neighborSolution);
            await Task.Delay(Utils.ValidationDelay).ConfigureAwait(false);

            if (Solver.IsMonitoringEnabled)
            {
               Provider.NotifyObservers(neighborSolution);
            }
         }

         await Task.WhenAll(Solver.AlgorithmTasks.Values);

         if (!currentNeighborhood.Any())
         {
            return;
         }
         var solution = GetBest(currentNeighborhood);

         Console.ForegroundColor = ConsoleColor.DarkGreen;
         SendMessage(MessageCode.LocalSearchNeighborhoodBest, solution.Id, solution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;

         _previousSolutionCost = CurrentBestSolution.Cost;

         bool isBetterThanCurrentBestSolution = Solver.Problem
            .CompareSolutionsCost(solution.Cost, CurrentBestSolution.Cost);

         if (!AcceptImprovementsOnly || 
            (AcceptImprovementsOnly && isBetterThanCurrentBestSolution))
         {
            CurrentBestSolution = solution;
         }

         if (isBetterThanCurrentBestSolution)
         {
            Console.ForegroundColor = ConsoleColor.Green;
            SendMessage(MessageCode.LocalSearchBestFound, solution.Cost, _previousSolutionCost);
            Console.ForegroundColor = ConsoleColor.Gray;

            _solutionsHistory.Add(solution);

            // E' migliore, ma di quanto? Se il delta di miglioramento è 0 comunque incremento l'iterationsWithoutImprovement.
            var deltaImprovement = CurrentBestSolution.Cost - _previousSolutionCost;
            if (deltaImprovement < _improvementThreshold)
            {
               _iterationsWithoutImprovement++;
               _shouldRunImprovement = _iterationsWithoutImprovement >= _maxIterationsWithoutImprovements;
            }
         }
         else
         {
            if (AcceptImprovementsOnly)
            {
               Console.ForegroundColor = ConsoleColor.Yellow;
               SendMessage(MessageCode.LocalSearchInvariateSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);
               Console.ForegroundColor = ConsoleColor.Gray;
            }
         }

         if (_canDoImprovements && _shouldRunImprovement)
         {
            await RunImprovementsInternal();
            _shouldRunImprovement = default;
            _iterationsWithoutImprovement = default;
            CurrentBestSolution = Solver.BestSolution;
            SendMessage(MessageCode.LocalSearchResumeSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);
         }
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = CurrentBestSolution;

         Console.ForegroundColor = ConsoleColor.Yellow;
         SendMessage(MessageCode.LocalSearchImprovementsPerformed, ImprovementsCount);
         SendMessage(MessageCode.LocalSearchStop, CurrentBestSolution.Id, CurrentBestSolution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;
         SendMessage(ToSolution.SolutionCollectionToString(_solutionsHistory));
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost == CurrentBestSolution.Cost || base.StopConditions();
      }
      #endregion
   }
}