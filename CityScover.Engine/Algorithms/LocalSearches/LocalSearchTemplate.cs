
//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/12/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CityScover.Commons;
using CityScover.Engine.Algorithms.CustomAlgorithms;
using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Algorithms.VariableDepthSearch;

namespace CityScover.Engine.Algorithms.LocalSearches
{
   internal class LocalSearchTemplate : Algorithm
   {
      #region Private fields
      private readonly NeighborhoodFacade<ToSolution> _neighborhoodFacade;
      private int _previousSolutionCost;
      private int _iterationsWithoutImprovement;
      private bool _shouldRunImprovementAlgorithm;
      private int _improvementThreshold;
      private int _maxIterationsWithoutImprovements;
      private ICollection<ToSolution> _solutionsHistory;
      #endregion

      #region Constructors
      internal LocalSearchTemplate(Neighborhood<ToSolution> neighborhood, AlgorithmTracker provider = null)
         : base(provider)
      {
         Type = neighborhood.Type;
         _neighborhoodFacade = new NeighborhoodFacade<ToSolution>(neighborhood);
      }
      #endregion

      #region Internal properties
      internal ToSolution CurrentBestSolution { get; set; }
      private bool CanDoImprovements { get; set; }
      #endregion

      #region Private methods
      private ToSolution GetBest(IEnumerable<ToSolution> neighborhood, ToSolution currentSolution, byte? maxImprovementsCount)
      {
         if (neighborhood is null || currentSolution is null)
         {
            throw new ArgumentNullException(nameof(neighborhood));
         }
         if (maxImprovementsCount.HasValue && maxImprovementsCount == 0)
         {
            throw new ArgumentException("maxImprovementsCount can not have value 0.");
         }

         ToSolution bestSolution = currentSolution;
         byte currentImprovement = default;

         foreach (var solution in neighborhood)
         {
            if (maxImprovementsCount.HasValue && currentImprovement > maxImprovementsCount)
            {
               break;
            }

            bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, bestSolution.Cost, true);
            if (isBetterThanCurrentBestSolution)
            {
               bestSolution = solution;
               currentImprovement++;
            }
         }

         return bestSolution;
      }

      private IEnumerable<Algorithm> GetImprovementAlgorithms()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms is null)
         {
            yield return null;
         }

         if (childrenAlgorithms is null)
         {
            yield break;
         }

         foreach (var child in childrenAlgorithms)
         {
            var algorithm = Solver.GetAlgorithm(child.CurrentAlgorithm);

            switch (algorithm)
            {
               case LinKernighan lk:
                  algorithm = lk;
                  lk.MaxSteps = child.RunningCount;
                  lk.CurrentBestSolution = CurrentBestSolution;
                  break;
               case HybridCustomInsertion hnd:
                  algorithm = hnd;
                  hnd.CurrentBestSolution = CurrentBestSolution;
                  break;
            }

            if (algorithm is null)
            {
               throw new NullReferenceException(nameof(algorithm));
            }
            algorithm.Parameters = child.AlgorithmParameters;
            algorithm.Provider = Provider;

            yield return algorithm;
         }
      }

      private async Task RunImprovementAlgorithms()
      {
         foreach (var algorithm in GetImprovementAlgorithms())
         {
            if (algorithm is null)
            {
               throw new InvalidOperationException("Bad configuration format: " +
                  $"{nameof(Solver.WorkingConfiguration)}.");
            }

            Solver.CurrentAlgorithm = algorithm.Type;
            Task improvementTask = Task.Run(algorithm.Start);

            try
            {
               await improvementTask.ConfigureAwait(false);
            }
            catch (AggregateException ae)
            {
               OnError(ae.InnerException);
            }
            finally
            {
               Solver.CurrentAlgorithm = Type;
               CurrentBestSolution = Solver.BestSolution;
               _shouldRunImprovementAlgorithm = false;
               _iterationsWithoutImprovement = 0;
            }

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
      internal void ResetState()
      {
         _previousSolutionCost = default;
         _iterationsWithoutImprovement = default;
         _shouldRunImprovementAlgorithm = default;
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
         CanDoImprovements = Parameters[ParameterCodes.CanDoImprovements];

         if (CanDoImprovements)
         {
            _improvementThreshold = Parameters[ParameterCodes.LSimprovementThreshold];
            _maxIterationsWithoutImprovements = Parameters[ParameterCodes.LSmaxRunsWithNoImprovements];
         }

         if (CurrentBestSolution is null)
         {
            CurrentBestSolution = Solver.BestSolution;
         }
         _solutionsHistory.Add(CurrentBestSolution);
         SendMessage(MessageCode.LSStartSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);
         _previousSolutionCost = default;
         _iterationsWithoutImprovement = default;
         _shouldRunImprovementAlgorithm = default;
      }

      protected override async Task PerformStep()
      {
         SendMessage(MessageCode.LSNewNeighborhood, CurrentStep);

         var currentNeighborhood = _neighborhoodFacade.GenerateNeighborhood(CurrentBestSolution);

         foreach (var neighborSolution in currentNeighborhood)
         {
            SendMessage(MessageCode.LSNewNeighborhoodMove, neighborSolution.Id, CurrentStep);
            SendMessage(neighborSolution.Description);
            Solver.EnqueueSolution(neighborSolution);
            await Task.Delay(Utils.DelayTask).ConfigureAwait(false);

            if (Solver.IsMonitoringEnabled)
            {
               Provider.NotifyObservers(neighborSolution);
            }
         }
         await Task.WhenAll(Solver.AlgorithmTasks.Values);

         // Cerco la migliore soluzione dell'intorno appena calcolato.
         var solution = GetBest(currentNeighborhood, CurrentBestSolution, null);
      
         Console.ForegroundColor = ConsoleColor.DarkGreen;
         SendMessage(MessageCode.LSNeighborhoodBest, solution.Id, solution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;

         _previousSolutionCost = CurrentBestSolution.Cost;

         bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, CurrentBestSolution.Cost);
         if (!AcceptImprovementsOnly || (AcceptImprovementsOnly && isBetterThanCurrentBestSolution))
         {
            CurrentBestSolution = solution;
         }

         if (isBetterThanCurrentBestSolution)
         {
            Console.ForegroundColor = ConsoleColor.Green;
            SendMessage(MessageCode.LSBestFound, solution.Cost, _previousSolutionCost);
            Console.ForegroundColor = ConsoleColor.Gray;
            _solutionsHistory.Add(solution);

            // E' migliore, ma di quanto? Se il delta è 0 comunque incremento l'iterationsWithoutImprovement.
            var delta = CurrentBestSolution.Cost - _previousSolutionCost;
            if (delta < _improvementThreshold)
            {
               _iterationsWithoutImprovement++;
               _shouldRunImprovementAlgorithm = _iterationsWithoutImprovement >= _maxIterationsWithoutImprovements;
            }
         }
         else
         {
            Console.ForegroundColor = ConsoleColor.Yellow;
            SendMessage(MessageCode.LSInvariateSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);
            Console.ForegroundColor = ConsoleColor.Gray;
         }

         if (CanDoImprovements && _shouldRunImprovementAlgorithm)
         {
            await RunImprovementAlgorithms();
            SendMessage(MessageCode.LSResumeSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);
         }
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = CurrentBestSolution;
         Console.ForegroundColor = ConsoleColor.Yellow;
         SendMessage(MessageCode.LSFinish, CurrentBestSolution.Id, CurrentBestSolution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;
         SendMessage(ToSolution.SolutionCollectionToString(_solutionsHistory));
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost == CurrentBestSolution.Cost ||
                base.StopConditions();
      }
      #endregion
   }
}