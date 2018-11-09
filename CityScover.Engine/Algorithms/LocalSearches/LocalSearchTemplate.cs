//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 08/11/2018
//

using CityScover.Engine.Algorithms.CustomAlgorithms;
using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Algorithms.VariableDepthSearch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms
{
   internal class LocalSearchTemplate : Algorithm
   {
      #region Private fields
      private int _previousSolutionCost;
      private int _currentSolutionCost;
      private byte _iterationsWithoutImprovement;
      private bool _shouldRunImprovementAlgorithm;
      private ushort _improvementThreshold;
      private byte _maxIterationsWithoutImprovements;
      private TOSolution _bestSolution;
      private NeighborhoodFacade _neighborhoodFacade;
      #endregion

      #region Constructors
      internal LocalSearchTemplate(Neighborhood neighborhood)
         : this(neighborhood, provider: null)
      {
      }

      public LocalSearchTemplate(Neighborhood neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
         _neighborhoodFacade = new NeighborhoodFacade(neighborhood, this);
      }
      #endregion

      #region Internal properties
      internal bool CanExecuteImprovementAlgorithms { get; set; }
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

      #region Old methods (TO DELETE)
      //private Algorithm GetImprovementAlgorithm()
      //{
      //   var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
      //   if (childrenAlgorithms is null)
      //   {
      //      return null;
      //   }

      //   var flow = childrenAlgorithms.FirstOrDefault();
      //   if (flow is null)
      //   {
      //      return null;
      //   }

      //   Algorithm algorithm = Solver.GetAlgorithm(flow.CurrentAlgorithm);

      //   if (algorithm is LinKernighan lk)
      //   {
      //      lk.MaxSteps = flow.RunningCount;
      //      lk.CurrentBestSolution = _bestSolution;
      //   }

      //   algorithm.Provider = Provider;
      //   return algorithm;
      //}

      //private async Task RunImprovementAlgorithm()
      //{
      //   Algorithm improvementAlgorithm = GetImprovementAlgorithm();
      //   if (improvementAlgorithm is null)
      //   {
      //      throw new InvalidOperationException($"Bad configuration format: " +
      //         $"{nameof(Solver.WorkingConfiguration)}.");
      //   }

      //   await Task.Run(() => improvementAlgorithm.Start());
      //   _shouldRunImprovementAlgorithm = false;
      //   _iterationsWithoutImprovement = 0;
      //}
      #endregion

      private IEnumerable<Algorithm> GetImprovementAlgorithms()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms is null)
         {
            yield return null;
         }

         Algorithm algorithm = default;
         foreach (var children in childrenAlgorithms)
         {
            algorithm = Solver.GetAlgorithm(children.CurrentAlgorithm);

            if (algorithm is LinKernighan lk)
            {
               lk.MaxSteps = children.RunningCount;
               lk.CurrentBestSolution = _bestSolution;
            }
            else if (algorithm is HybridNearestDistance hnd)
            {
               // TODO
               // ...
            }

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

            await Task.Run(() => algorithm.Start());
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
         if (Solver.IsMonitoringEnabled)
         {
            SendMessage(MessageCodes.StageStart, Solver.CurrentStage.Description);
         }

         Solver.PreviousStageSolutionCost = Solver.BestSolution.Cost;
         _improvementThreshold = Solver.CurrentStage.Flow.LkImprovementThreshold;
         _maxIterationsWithoutImprovements = Solver.CurrentStage.Flow.MaxIterationsWithoutImprovements;
         CanExecuteImprovementAlgorithms = Solver.CurrentStage.Flow.CanExecuteImprovements;
         _bestSolution = Solver.BestSolution;
         _currentSolutionCost = _bestSolution.Cost;
         _previousSolutionCost = default;
         _iterationsWithoutImprovement = default;
         _shouldRunImprovementAlgorithm = default;
      }

      internal override async Task PerformStep()
      {
         if (CanExecuteImprovementAlgorithms && _shouldRunImprovementAlgorithm)
         {
            await RunImprovementAlgorithms();
         }

         var currentNeighborhood = _neighborhoodFacade.GenerateNeighborhood(_bestSolution);

         foreach (var neighborSolution in currentNeighborhood)
         {
            SendMessage(MessageCodes.LSNewNeighborhoodMove, neighborSolution.Id, CurrentStep + 1);
            SendMessage(neighborSolution.Description);
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
         SendMessage(MessageCodes.LSNeighborhoodBest, solution.Id, solution.Cost);

         _previousSolutionCost = _currentSolutionCost;

         if (AcceptImprovementsOnly)
         {
            bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, _bestSolution.Cost);
            if (isBetterThanCurrentBestSolution)
            {
               SendMessage(MessageCodes.LSBestFound, solution.Cost, _bestSolution.Cost);
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
         SendMessage(MessageCodes.LSFinish, _currentSolutionCost);
      }

      internal override void OnTerminated()
      {
         Result validResult =
            new Result(_bestSolution, CurrentAlgorithm, null, Result.Validity.Valid);
         validResult.ResultFamily = AlgorithmFamily.LocalSearch;
         Solver.Results.Add(validResult);
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost == _currentSolutionCost ||
            Status == AlgorithmStatus.Error;
      }
      #endregion
   }
}