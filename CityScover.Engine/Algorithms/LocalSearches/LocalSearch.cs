//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 2010/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Algorithms.VariableDepthSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms
{
   internal class LocalSearch : Algorithm
   {
      #region Private fields
      private int _previousSolutionCost;
      private int _currentSolutionCost;
      private byte _iterationsWithoutImprovement;
      private bool _shouldRunImprovementAlgorithm;
      private byte _improvementThreshold;
      private byte _maxIterationsWithoutImprovements;
      private TOSolution _bestSolution;
      private NeighborhoodFacade _neighborhoodFacade;
      #endregion

      #region Constructors
      internal LocalSearch(Neighborhood neighborhood)
         : this(neighborhood, null)
      {
      }

      public LocalSearch(Neighborhood neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
         _neighborhoodFacade = new NeighborhoodFacade(neighborhood);
      }
      #endregion

      #region Private methods
      private TOSolution GetBest(IEnumerable<TOSolution> neighborhood, TOSolution currentSolution, byte? maxImprovementsCount)
      {
         if (neighborhood == null || currentSolution == null)
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

      private Algorithm GetImprovementAlgorithm()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms == null)
         {
            return null;
         }

         var flow = childrenAlgorithms.FirstOrDefault();
         if (flow == null)
         {
            return null;
         }

         Algorithm algorithm = Solver.GetAlgorithm(flow.CurrentAlgorithm);

         if (algorithm is LinKernighan lk)
         {
            lk.MaxSteps = flow.RunningCount;
            lk.CurrentBestSolution = _bestSolution;
         }

         return algorithm;
      }

      private async Task RunImprovementLogic()
      {
         Algorithm improvementAlgorithm = GetImprovementAlgorithm();
         if (improvementAlgorithm == null)
         {
            throw new InvalidOperationException($"Bad configuration format: " +
               $"{nameof(Solver.WorkingConfiguration)}.");
         }

         await improvementAlgorithm.Start();
         ClearState();
      }

      private void ClearState()
      {
         _iterationsWithoutImprovement = 0;
         _shouldRunImprovementAlgorithm = false;
      }
      #endregion

      #region Internal properties
      internal bool CanExecuteImprovementAlgorithms { get; set; }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         Solver.PreviousStageSolutionCost = Solver.BestSolution.Cost;
         _improvementThreshold = Solver.CurrentStage.Flow.ImprovementThreshold;
         _maxIterationsWithoutImprovements = Solver.CurrentStage.Flow.MaxIterationsWithImprovements;
         CanExecuteImprovementAlgorithms = Solver.CurrentStage.Flow.CanExecuteImprovements;
         _bestSolution = Solver.BestSolution;
         _currentSolutionCost = _bestSolution.Cost;
         _previousSolutionCost = default;
         _iterationsWithoutImprovement = default;
         _shouldRunImprovementAlgorithm = default;
      }

      internal override async Task PerformStep()
      {
         if (CanExecuteImprovementAlgorithms &&
            _shouldRunImprovementAlgorithm)
         {
            Task improvementTask = Task.Run(() => RunImprovementLogic());
            await improvementTask;
         }

         var currentNeighborhood = _neighborhoodFacade.GenerateNeighborhood(_bestSolution);

         foreach (var neighborSolution in currentNeighborhood)
         {
            Solver.EnqueueSolution(neighborSolution);
            await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);

            if (Solver.IsMonitoringEnabled)
            {
               // Notifica gli observers.
               Provider.NotifyObservers(neighborSolution);
            }
         }
         await Task.WhenAll(Solver.AlgorithmTasks.Values);
         var solution = GetBest(currentNeighborhood, _bestSolution, null);
         _previousSolutionCost = _currentSolutionCost;

         if (AcceptImprovementsOnly)
         {
            bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, _bestSolution.Cost);
            if (isBetterThanCurrentBestSolution)
            {
               _bestSolution = solution;
               _currentSolutionCost = solution.Cost;

               if (CanExecuteImprovementAlgorithms)
               {
                  var delta = _currentSolutionCost - _previousSolutionCost;
                  if (delta < _improvementThreshold)
                  {
                     _iterationsWithoutImprovement++;
                     _shouldRunImprovementAlgorithm = _iterationsWithoutImprovement >= _maxIterationsWithoutImprovements;
                  } 
               }
            }
         }
         else
         {
            _bestSolution = solution;
            _currentSolutionCost = solution.Cost;
         }
      }

      internal override void OnError(Exception exception)
      {
         // Da gestire timeSpent (probabilmente con metodo che somma i tempi di tutti i nodi).
         Result resultError =
            new Result(_bestSolution, CurrentAlgorithm, null, Result.Validity.Invalid);
         resultError.ResultFamily = AlgorithmFamily.LocalSearch;
         Solver.Results.Add(resultError);
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _bestSolution;
      }

      internal override void OnTerminated()
      {
         // Da gestire timeSpent (probabilmente con metodo che somma i tempi di tutti i nodi).
         Result validResult =
            new Result(_bestSolution, CurrentAlgorithm, null, Result.Validity.Valid);
         validResult.ResultFamily = AlgorithmFamily.LocalSearch;
         Solver.Results.Add(validResult);
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost == _currentSolutionCost ||
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}