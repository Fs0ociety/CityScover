
//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/11/2018
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
      private readonly NeighborhoodFacade _neighborhoodFacade;
      private int _previousSolutionCost;
      private int _iterationsWithoutImprovement;
      private bool _shouldRunImprovementAlgorithm;
      private int _improvementThreshold;
      private int _maxIterationsWithoutImprovements;
      private ICollection<TOSolution> _solutionsHistory;
      #endregion

      #region Constructors
      internal LocalSearchTemplate(Neighborhood neighborhood, AlgorithmTracker provider = null)
         : base(provider)
      {
         Type = neighborhood.Type;
         _neighborhoodFacade = new NeighborhoodFacade(neighborhood, this);
      }
      #endregion

      #region Internal properties
      internal TOSolution CurrentBestSolution { get; set; }
      private bool CanDoImprovements { get; set; }

      /// <summary>
      /// Property used to assign the local search move which has made this solution.
      /// Used only by Tabu Search.
      /// </summary>
      internal Tuple<int, int> Move { get; set; }
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

      #region Private methods
      private TOSolution GetBest(IEnumerable<TOSolution> neighborhood, TOSolution currentSolution, byte? maxImprovementsCount)
      {
         if (neighborhood is null || currentSolution is null)
         {
            throw new ArgumentNullException(nameof(neighborhood));
         }
         if (maxImprovementsCount.HasValue && maxImprovementsCount == 0)
         {
            throw new ArgumentException("maxImprovementsCount can not have value 0.");
         }

         TOSolution bestSolution = currentSolution;
         byte currentImprovement = default;

         foreach (var solution in neighborhood)
         {
            if (maxImprovementsCount.HasValue && currentImprovement > maxImprovementsCount)
            {
               break;
            }

            bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, bestSolution.Cost);
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
               throw new InvalidOperationException($"Bad configuration format: " +
                  $"{nameof(Solver.WorkingConfiguration)}.");
            }

            Solver.CurrentAlgorithm = algorithm.Type;
            await Task.Run(() => algorithm.Start());
            Solver.CurrentAlgorithm = Type;
            CurrentBestSolution = Solver.BestSolution;
            _shouldRunImprovementAlgorithm = false;
            _iterationsWithoutImprovement = 0;

            // Verificare se necessario eseguire i successivi algoritmi dello StageFlow presenti tra i ChildrenFlows.
            // Usare un if-break per questo scopo sotto questo commento, dopo la terminazione dell'algoritmo.
            // Senza questo if verrebbero eseguiti a prescindere in sequenza tutti gli algoritmi presenti tra i ChildrenFlows, 
            // anche se non fosse necessario avviare i successivi.
         }
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _solutionsHistory = new Collection<TOSolution>();
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
         var currentNeighborhood = _neighborhoodFacade.GenerateNeighborhood(CurrentBestSolution, NeighborhoodFacade.RunningMode.Parallel);

         foreach (var neighborSolution in currentNeighborhood)
         {
            SendMessage(MessageCode.LSNewNeighborhoodMove, neighborSolution.Id, CurrentStep);
            SendMessage(neighborSolution.Description);
            Solver.EnqueueSolution(neighborSolution);
            await Task.Delay(Utils.DelayTask).ConfigureAwait(false);

            if (Solver.IsMonitoringEnabled)
            {
               // Notifica gli observers.
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

            //E' migliore, ma di quanto? Se il delta è 0 comunque incremento l'iterationsWithoutImprovement.
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

         #region TODO: REMOVE Old-LocalSearch
         //if (AcceptImprovementsOnly)
         //{
         //   bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, CurrentBestSolution.Cost);
         //   if (isBetterThanCurrentBestSolution)
         //   {
         //      SendMessage(MessageCode.LSBestFound, solution.Cost, CurrentBestSolution.Cost);
         //      CurrentBestSolution = solution;
         //      _solutionsHistory.Add(solution);
         //      _currentSolutionCost = solution.Cost;

         //      var delta = _currentSolutionCost - _previousSolutionCost;
         //      if (CanDoImprovements && delta < _improvementThreshold)
         //      {
         //         _iterationsWithoutImprovement++;
         //         _shouldRunImprovementAlgorithm = _iterationsWithoutImprovement >= _maxIterationsWithoutImprovements;
         //      }
         //   }
         //   else
         //   {
         //      _iterationsWithoutImprovement++;
         //   }
         //}
         //else
         //{
         //   CurrentBestSolution = solution;
         //   _currentSolutionCost = solution.Cost;

         //   var delta = _currentSolutionCost - _previousSolutionCost;
         //   if (CanDoImprovements && delta < _improvementThreshold)
         //   {
         //      _iterationsWithoutImprovement++;
         //      _shouldRunImprovementAlgorithm = _iterationsWithoutImprovement >= _maxIterationsWithoutImprovements;
         //   }
         //}
         #endregion

         if (CanDoImprovements && _shouldRunImprovementAlgorithm)
         {
            Task improvementTask = RunImprovementAlgorithms();
            try
            {
               improvementTask.Wait();
            }
            catch (AggregateException ae)
            {
               OnError(ae.InnerException);
            }
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
         SendMessage(TOSolution.SolutionCollectionToString(_solutionsHistory));
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost == CurrentBestSolution.Cost || base.StopConditions();
      }
      #endregion
   }
}