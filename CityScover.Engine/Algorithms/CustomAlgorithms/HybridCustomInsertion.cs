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

using CityScover.Commons;
using CityScover.Engine.Workers;
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
      private int _addedNodesCount;
      private int _previousEndPoiKey;
      private DateTime _tMax;
      private DateTime _tMaxThresholdTime;
      private TimeSpan _tMaxThreshold;
      private TOSolution _currentSolution;
      #endregion

      #region Constructors
      internal HybridCustomInsertion()
         : this(provider: null)
      {
      }

      internal HybridCustomInsertion(AlgorithmTracker provider)
         : base(provider)
      {
         Type = AlgorithmType.HybridCustomInsertion;
      }
      #endregion

      #region Private Protected members
      private protected CityMapGraph Tour;
      private protected InterestPointWorker StartPoi;
      private protected InterestPointWorker EndPoi;
      private protected TimeSpan TimeWalkThreshold;
      private protected Queue<InterestPointWorker> ProcessingNodes;
      private protected ICollection<TOSolution> SolutionsHistory;

      private protected void AddPointsNotInTour()
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
      #endregion

      #region Internal properties
      internal TOSolution CurrentBestSolution { get; set; }
      #endregion

      #region Private methods
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

      private void Restart()
      {
         Algorithm algorithm = Solver.GetAlgorithm(AlgorithmType.HybridCustomInsertion);
         algorithm.Provider = Provider;
         algorithm.Parameters = Parameters;

         Task algorithmTask = Task.Run(() => algorithm.Start());
         try
         {
            algorithmTask.Wait();
         }
         catch (AggregateException ae)
         {
            OnError(ae.InnerException);
         }
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         SolutionsHistory = new Collection<TOSolution>();

         if (!Parameters.Any())
         {
            throw new KeyNotFoundException(nameof(Parameters));
         }
         if (Solver.IsMonitoringEnabled && Type == AlgorithmType.HybridCustomInsertion)
         {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            SendMessage(MessageCode.HDIStarting);
            Console.ForegroundColor = ConsoleColor.Gray;
         }
         if (CurrentBestSolution is null)
         {
            CurrentBestSolution = Solver.BestSolution;
         }

         ProcessingNodes = new Queue<InterestPointWorker>();
         Solver.PreviousStageSolutionCost = CurrentBestSolution.Cost;
         Tour = CurrentBestSolution.SolutionGraph.DeepCopy();
         _tMaxThreshold = Parameters[ParameterCodes.HDIthresholdToTmax];
         TimeWalkThreshold = Parameters[ParameterCodes.HDItimeWalkThreshold];
         _tMax = Solver.WorkingConfiguration.ArrivalTime
            .Add(Solver.WorkingConfiguration.TourDuration);
         _tMaxThresholdTime = _tMax - _tMaxThreshold;

         StartPoi = Tour.GetStartPoint() ?? throw
            new NullReferenceException(nameof(StartPoi));
         EndPoi = Tour.GetEndPoint() ?? throw
            new NullReferenceException(nameof(EndPoi));

         AddPointsNotInTour();
         _addedNodesCount = default;
      }

      internal override async Task PerformStep()
      {
         InterestPointWorker point = ProcessingNodes.Dequeue();

         TryAddNode(point, EndPoi.Entity.Id, StartPoi.Entity.Id);
         _previousEndPoiKey = EndPoi.Entity.Id;
         EndPoi = Tour.GetEndPoint();
         _addedNodesCount++;

         _currentSolution = new TOSolution()
         {
            SolutionGraph = Tour.DeepCopy()
         };

         Solver.EnqueueSolution(_currentSolution);
         await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[_currentSolution.Id];
         SendMessage(MessageCode.HDINewNodeAdded, point.Entity.Name);
         SolutionsHistory.Add(_currentSolution);

         if (!_currentSolution.IsValid)
         {
            UndoAdditionPoint(point.Entity.Id, _previousEndPoiKey, StartPoi.Entity.Id);
            _addedNodesCount--;
            EndPoi = Tour.GetEndPoint();
            SendMessage(MessageCode.HDINewNodeRemoved, point.Entity.Name);
         }

         // Notify observers.
         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(_currentSolution);
         }
      }

      internal override void OnError(Exception exception)
      {
         ProcessingNodes.Clear();
         ProcessingNodes = null;
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         SendMessage(TOSolution.SolutionCollectionToString(SolutionsHistory));

         if (_addedNodesCount == 0)
         {
            HybridCustomUpdate updateAlgorithm = 
               (HybridCustomUpdate)Solver.GetAlgorithm(AlgorithmType.HybridCustomUpdate);
            updateAlgorithm.Provider = Provider;
            updateAlgorithm.Parameters = Parameters;

            if (Solver.IsMonitoringEnabled)
            {
               Console.ForegroundColor = ConsoleColor.DarkMagenta;
               SendMessage(MessageCode.HDUStarting);
               Console.ForegroundColor = ConsoleColor.Gray;
            }

            Task updateTask = Task.Run(() => updateAlgorithm.Start());
            try
            {
               updateTask.Wait();
            }
            catch (AggregateException ae)
            {
               OnError(ae.InnerException);
            }
            finally
            {
               ProcessingNodes.Clear();
               ProcessingNodes = null;
            }

            if (updateAlgorithm.TourUpdated)
            {
               bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(
               updateAlgorithm.CurrentSolution.Cost, Solver.BestSolution.Cost, considerEqualityComparison: true);

               if (isBetterThanCurrentBestSolution)
               {
                  Solver.BestSolution = updateAlgorithm.CurrentSolution;
                  Restart();
               }
            }
            updateAlgorithm = null;
         }
         else
         {
            Solver.BestSolution = _currentSolution;
         }
      }

      internal override bool StopConditions()
      {
         bool isGreaterThanTmaxThreshold = EndPoi.TotalTime > _tMaxThresholdTime;

         bool shouldStop = isGreaterThanTmaxThreshold || !ProcessingNodes.Any() ||
            Status == AlgorithmStatus.Error;

         return shouldStop;
      }
      #endregion
   }
}