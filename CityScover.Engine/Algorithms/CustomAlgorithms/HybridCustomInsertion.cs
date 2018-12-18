//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 18/12/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.CustomAlgorithms
{
   internal class HybridCustomInsertion : Algorithm
   {
      #region Private fields
      private DateTime _tMax;
      private DateTime _tMaxThresholdTime;
      private TimeSpan _timeThresholdToTmax;
      private ToSolution _currentSolution;
      private int _addedNodesCount;
      #endregion

      #region Constructors
      internal HybridCustomInsertion() : this(provider: null)
      {
      }

      internal HybridCustomInsertion(AlgorithmTracker provider) : base(provider)
         => Type = AlgorithmType.HybridCustomInsertion;
      #endregion

      #region Private Protected members
      private protected CityMapGraph Tour;
      private protected InterestPointWorker StartPoi;
      private protected InterestPointWorker EndPoi;
      private protected Queue<InterestPointWorker> ProcessingNodes;
      private protected ICollection<ToSolution> SolutionsHistory;
      #endregion

      #region Internal properties
      internal ToSolution CurrentBestSolution { get; set; }
      #endregion

      #region Private methods
      private void AddPointsNotInTour()
      {
         var currentSolutionNodes = Tour.Nodes;
         var cityMapGraphNodes = Solver.CityMapGraph.TourPoints;

         var cityMapGraphNodeIds = cityMapGraphNodes.Select(point => point.Entity.Id);
         var currentSolutionNodeIds = currentSolutionNodes.Select(point => point.Entity.Id);
         var filteredNodeIds = cityMapGraphNodeIds.Except(currentSolutionNodeIds);

         var orderedFilteredNodes = cityMapGraphNodes
            .Where(point => filteredNodeIds.Any(nodeId => nodeId == point.Entity.Id))
            .OrderByDescending(point => point.Entity.Score.Value);

         orderedFilteredNodes.ToList().ForEach(node => ProcessingNodes.Enqueue(node));
      }

      private void TryAddNode(InterestPointWorker nodeToAdd, int fromNodeKey, int toNodeKey)
      {
         int nodeKeyToAdd = nodeToAdd.Entity.Id;

         Tour.RemoveEdge(fromNodeKey, toNodeKey);
         Tour.AddNode(nodeKeyToAdd, nodeToAdd);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, fromNodeKey, nodeKeyToAdd);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, nodeKeyToAdd, toNodeKey);
      }

      private void UndoAdditionPoint(int nodeKeyToRemove, int fromNodeKey, int toNodeKey)
      {
         Tour.RemoveEdge(nodeKeyToRemove, toNodeKey);
         Tour.RemoveEdge(fromNodeKey, nodeKeyToRemove);
         Tour.RemoveNode(nodeKeyToRemove);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, fromNodeKey, toNodeKey);
      }

      private HybridCustomUpdate RunUpdateTour()
      {
         var updateAlgorithm = (HybridCustomUpdate)Solver.GetAlgorithm(
            AlgorithmType.HybridCustomUpdate);

         if (updateAlgorithm is null)
         {
            throw new NullReferenceException(nameof(updateAlgorithm));
         }

         updateAlgorithm.Provider = Provider;
         updateAlgorithm.Parameters = Parameters;

         Solver.CurrentAlgorithm = updateAlgorithm.Type;
         Task updateTask = Task.Run(updateAlgorithm.Start);
         try
         {
            updateTask.Wait();
         }
         catch (AggregateException ae)
         {
            updateAlgorithm = null;
            OnError(ae.InnerException);
         }
         finally
         {
            Solver.CurrentAlgorithm = Type;
         }

         return updateAlgorithm;
      }

      #region TODO: Remove method Restart()
      //private void Restart()
      //{
      //   Algorithm algorithm = Solver.GetAlgorithm(AlgorithmType.HybridCustomInsertion);

      //   if (algorithm is null)
      //   {
      //      throw new NullReferenceException(nameof(algorithm));
      //   }
   
      //   algorithm.Provider = Provider;
      //   algorithm.Parameters = Parameters;

      //   Task algorithmTask = Task.Run(() => algorithm.Start());
      //   try
      //   {
      //      algorithmTask.Wait();
      //   }
      //   catch (AggregateException ae)
      //   {
      //      OnError(ae.InnerException);
      //   }
      //}
      #endregion

      private bool IsTimeThresholdToTmaxSatisfied(ToSolution solution)
      {
         var endPoint = solution.SolutionGraph.GetEndPoint();
         bool isSatisfied = endPoint.TotalTime <= _tMaxThresholdTime;
         return isSatisfied;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         SolutionsHistory = new Collection<ToSolution>();

         if (!Parameters.Any())
         {
            throw new KeyNotFoundException(nameof(Parameters));
         }
         if (Solver.IsMonitoringEnabled && Type == AlgorithmType.HybridCustomInsertion)
         {
            if (!Parameters.ContainsKey(ParameterCodes.HciTimeThresholdToTmax))
            {
               throw new KeyNotFoundException(nameof(ParameterCodes.HciTimeThresholdToTmax));
            }

            _timeThresholdToTmax = Parameters[ParameterCodes.HciTimeThresholdToTmax];
            _tMax = Solver.WorkingConfiguration.ArrivalTime.Add(Solver.WorkingConfiguration.TourDuration);
            _tMaxThresholdTime = _tMax - _timeThresholdToTmax;

            //Solver.Problem.Constraints.Add(
            //   new KeyValuePair<string, Func<ToSolution, bool>>(
            //      "TimeThresholdToTmax", IsTimeThresholdToTmaxSatisfied));

            Console.ForegroundColor = ConsoleColor.Yellow;
            SendMessage(MessageCode.HybridCustomInsertionStart);
            Console.ForegroundColor = ConsoleColor.Gray;
         }

         if (CurrentBestSolution is null)
         {
            CurrentBestSolution = Solver.BestSolution;
         }

         _addedNodesCount = default;
         ProcessingNodes = new Queue<InterestPointWorker>();
         Tour = CurrentBestSolution.SolutionGraph.DeepCopy();
         StartPoi = Tour.GetStartPoint() ?? throw new NullReferenceException(nameof(StartPoi));
         EndPoi = Tour.GetEndPoint() ?? throw new NullReferenceException(nameof(EndPoi));
         Solver.PreviousStageSolutionCost = CurrentBestSolution.Cost;
         AddPointsNotInTour();
      }

      protected override async Task PerformStep()
      {
         InterestPointWorker point = ProcessingNodes.Dequeue();

         TryAddNode(point, EndPoi.Entity.Id, StartPoi.Entity.Id);
         var previousEndPoiKey = EndPoi.Entity.Id;
         EndPoi = Tour.GetEndPoint();
         _addedNodesCount++;

         _currentSolution = new ToSolution()
         {
            SolutionGraph = Tour.DeepCopy()
         };

         Solver.EnqueueSolution(_currentSolution);
         await Task.Delay(Utils.ValidationDelay).ConfigureAwait(false);
         await Solver.AlgorithmTasks[_currentSolution.Id];
         SendMessage(MessageCode.HybridCustomInsertionNewNodeAdded,
            point.Entity.Name);
         SolutionsHistory.Add(_currentSolution);

         if (!_currentSolution.IsValid)
         {
            UndoAdditionPoint(point.Entity.Id, previousEndPoiKey, StartPoi.Entity.Id);
            EndPoi = Tour.GetEndPoint();
            _addedNodesCount--;
            SendMessage(MessageCode.HybridCustomInsertionNewNodeRemoved,
               point.Entity.Name);
         }

         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(_currentSolution);
         }
      }

      internal override void OnError(Exception exception)
      {
         ProcessingNodes?.Clear();
         ProcessingNodes = null;
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();

         // TODO
         // Remove the "timeThresholdToTmax" constraint from Constraints collection of Problem.

         Console.ForegroundColor = ConsoleColor.Yellow;
         SendMessage(MessageCode.HybridCustomInsertionStopWithSolution, _currentSolution.Id, _currentSolution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;

         if (_addedNodesCount == 0)
         {
            var updateAlgorithm = RunUpdateTour();
            if (updateAlgorithm.TourUpdated == false)
            {
               Console.ForegroundColor = ConsoleColor.Yellow;
               SendMessage(MessageCode.HybridCustomInsertionStopWithoutSolution);
               Console.ForegroundColor = ConsoleColor.Gray;

               return;
            }

            var isBetterThanCurrentBestSolution = Solver.Problem
               .CompareSolutionsCost(updateAlgorithm.CurrentSolution.Cost, Solver.BestSolution.Cost, true);

            if (!isBetterThanCurrentBestSolution)
            {
               Console.ForegroundColor = ConsoleColor.Yellow;
               SendMessage(MessageCode.HybridCustomUpdateStopWithSolution,
                  updateAlgorithm.CurrentSolution.Id, updateAlgorithm.CurrentSolution.Cost);
               Console.ForegroundColor = ConsoleColor.Gray;

               return;
            }
         }
         else
         {
            // Surely exists valid solutions here!
            var bestSolution = SolutionsHistory
               .Where(solution => solution.IsValid)
               .MaxBy(solution => solution.Cost);

            var (PreviousSolutionId, PreviousSolutionCost) = (Solver.BestSolution.Id, Solver.BestSolution.Cost);
            Solver.BestSolution = bestSolution;

            Console.ForegroundColor = ConsoleColor.Yellow;
            SendMessage(MessageCode.HybridCustomInsertionFinalSolution, 
               bestSolution.Id, bestSolution.Cost, PreviousSolutionId, PreviousSolutionCost);
            Console.ForegroundColor = ConsoleColor.Gray;
            SendMessage(ToSolution.SolutionCollectionToString(SolutionsHistory));
         }
      }

      internal override void OnTerminated()
      {
         SolutionsHistory?.Clear();
         ProcessingNodes?.Clear();
         SolutionsHistory = null;
         ProcessingNodes = null;
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         //bool isGreaterThanTmaxThreshold = EndPoi.TotalTime > _tMaxThresholdTime;
         //bool shouldStop = isGreaterThanTmaxThreshold || !ProcessingNodes.Any() ||
         //   Status == AlgorithmStatus.Error;

         bool shouldStop = !ProcessingNodes.Any() || Status == AlgorithmStatus.Error;
         return shouldStop;
      }
      #endregion
   }
}