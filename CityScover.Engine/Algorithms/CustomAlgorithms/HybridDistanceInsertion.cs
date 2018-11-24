//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 24/11/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.CustomAlgorithms
{
   internal class HybridDistanceInsertion : Algorithm
   {
      #region Private fields
      private int _addedNodesCount;
      private int _previousEndPOIKey;
      private DateTime _tMax;
      private DateTime _tMaxThresholdTime;
      private TimeSpan _tMaxThreshold;
      #endregion

      #region Constructors
      internal HybridDistanceInsertion()
         : this(provider: null)
      {
      }

      internal HybridDistanceInsertion(AlgorithmTracker provider)
         : base(provider)
      {
         Type = AlgorithmType.HybridDistanceInsertion;
      }
      #endregion

      #region Private Protected members
      private protected CityMapGraph _tour;
      private protected InterestPointWorker _startPOI;
      private protected InterestPointWorker _endPOI;
      private protected TimeSpan _timeWalkThreshold;
      private protected Queue<InterestPointWorker> _processingNodes;
      private protected TOSolution _currentSolution;

      private protected void AddPointsNotInTour()
      {
         IEnumerable<InterestPointWorker> currentSolutionNodes = _tour.Nodes;
         IEnumerable<InterestPointWorker> cityMapGraphNodes = Solver.CityMapGraph.TourPoints;

         var cityMapGraphNodeIds = cityMapGraphNodes.Select(point => point.Entity.Id);
         var currentSolutionNodeIds = currentSolutionNodes.Select(point => point.Entity.Id);
         var filteredNodeIds = cityMapGraphNodeIds.Except(currentSolutionNodeIds);

         var orderedFilteredNodes = cityMapGraphNodes
            .Where(point => filteredNodeIds.Where(nodeId => nodeId == point.Entity.Id).Any())
            .OrderByDescending(point => point.Entity.Score.Value);

         orderedFilteredNodes.ToList().ForEach(node => _processingNodes.Enqueue(node));
      }
      #endregion

      #region Internal properties
      internal TOSolution CurrentBestSolution { get; set; }
      #endregion

      #region Private methods
      private void TryAddNode(InterestPointWorker nodeToAdd, int fromNodeKey, int toNodeKey)
      {
         int nodeKeyToAdd = nodeToAdd.Entity.Id;

         _tour.RemoveEdge(fromNodeKey, toNodeKey);
         _tour.AddNode(nodeKeyToAdd, nodeToAdd);
         _tour.AddRouteFromGraph(Solver.CityMapGraph, fromNodeKey, nodeKeyToAdd);
         _tour.AddRouteFromGraph(Solver.CityMapGraph, nodeKeyToAdd, toNodeKey);
      }

      private void UndoAdditionPoint(int nodeKeyToRemove, int fromNodeKey, int toNodeKey)
      {
         _tour.RemoveEdge(nodeKeyToRemove, toNodeKey);
         _tour.RemoveEdge(fromNodeKey, nodeKeyToRemove);
         _tour.RemoveNode(nodeKeyToRemove);
         _tour.AddRouteFromGraph(Solver.CityMapGraph, fromNodeKey, toNodeKey);
      }

      private void Restart()
      {
         AddPointsNotInTour();
         Task insertionTask = Task.Run(() => PerformStep());

         try
         {
            insertionTask.Wait();
         }
         catch (AggregateException ae)
         {
            OnError(ae.InnerException);
         }

         //Algorithm algorithm = new HybridDistanceInsertion
         //{
         //   Provider = Provider,
         //   Parameters = Parameters
         //};

         //Task algorithmTask = Task.Run(() => algorithm.Start());
         //try
         //{
         //   algorithmTask.Wait();
         //}
         //catch (AggregateException ae)
         //{
         //   OnError(ae.InnerException);
         //}
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

         if (!Parameters.Any())
         {
            throw new KeyNotFoundException(nameof(Parameters));
         }
         if (Solver.IsMonitoringEnabled)
         {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            SendMessage(MessageCode.HDIStarting);
            Console.ForegroundColor = ConsoleColor.Gray;
         }
         if (CurrentBestSolution is null)
         {
            CurrentBestSolution = Solver.BestSolution;
         }

         _processingNodes = new Queue<InterestPointWorker>();
         Solver.PreviousStageSolutionCost = CurrentBestSolution.Cost;
         _tour = CurrentBestSolution.SolutionGraph.DeepCopy();         
         _tMaxThreshold = Parameters[ParameterCodes.HDIthresholdToTmax];
         _timeWalkThreshold = Parameters[ParameterCodes.HDItimeWalkThreshold];
         _tMax = Solver.WorkingConfiguration.ArrivalTime
            .Add(Solver.WorkingConfiguration.TourDuration);
         _tMaxThresholdTime = _tMax - _tMaxThreshold;

         _startPOI = _tour.GetStartPoint() ?? throw
            new NullReferenceException(nameof(_startPOI));
         _endPOI = _tour.GetEndPoint() ?? throw
            new NullReferenceException(nameof(_endPOI));

         AddPointsNotInTour();
         _addedNodesCount = default;
      }

      internal override async Task PerformStep()
      {
         InterestPointWorker point = _processingNodes.Dequeue();

         TryAddNode(point, _endPOI.Entity.Id, _startPOI.Entity.Id);
         _previousEndPOIKey = _endPOI.Entity.Id;
         _endPOI = _tour.GetEndPoint();
         _addedNodesCount++;

         _currentSolution = new TOSolution()
         {
            SolutionGraph = _tour
         };

         SendMessage(MessageCode.HDINewNodeAdded, point.Entity.Name);
         Solver.EnqueueSolution(_currentSolution);
         await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[_currentSolution.Id];

         if (!_currentSolution.IsValid)
         {
            UndoAdditionPoint(point.Entity.Id, _previousEndPOIKey, _startPOI.Entity.Id);
            _addedNodesCount--;
            _endPOI = _tour.GetEndPoint();
            SendMessage(MessageCode.HDINewNodeRemoved, point.Entity.Name);
         }
      }

      internal override void OnError(Exception exception)
      {
         _processingNodes.Clear();
         _processingNodes = null;
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();

         if (_addedNodesCount == 0)
         {
            HybridDistanceUpdate updateAlgorithm = new HybridDistanceUpdate()
            {
               Provider = Provider,
               Parameters = Parameters
            };

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
               _processingNodes.Clear();
               _processingNodes = null;
            }

            if (updateAlgorithm.TourUpdated)
            {
               if (_currentSolution != null && Solver.BestSolution != null)
               {
                  bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(
                  _currentSolution.Cost, Solver.BestSolution.Cost, considerEqualityComparison: true);

                  if (isBetterThanCurrentBestSolution)
                  {
                     Solver.BestSolution = _currentSolution;
                     Restart();
                  }
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
         bool isGreaterThanTmaxThreshold = _endPOI.TotalTime > _tMaxThresholdTime;

         bool shouldStop = isGreaterThanTmaxThreshold || !_processingNodes.Any() ||
            Status == AlgorithmStatus.Error;

         return shouldStop;
      }
      #endregion
   }
}