//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 11/11/2018
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
   internal class HybridNearestDistance : Algorithm
   {
      #region Private fields
      private int _addedNodesCount;
      private int _previousEndPOIKey;
      private bool _isTourUpdated;
      private InterestPointWorker _startPOI;
      private InterestPointWorker _endPOI;
      private DateTime _tMax;
      private DateTime _tMaxThresholdTime;
      private TimeSpan _tMaxThreshold;
      private TimeSpan _timeWalkThreshold;
      private TOSolution _currentSolution;
      private CityMapGraph _tour;
      private Queue<InterestPointWorker> _processingNodes;
      #endregion

      #region Constructors
      internal HybridNearestDistance()
         : this(null)
      {
      }

      internal HybridNearestDistance(AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      private void AddPointsNotInTour()
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

      private IEnumerable<(RouteWorker edge, TimeSpan tWalk)> CalculateMaximumEdgesTimeWalk(double averageSpeedWalk)
      {
         // Insieme delle tuple contenenti le informazioni: (Tratta, Tempo di percorrenza).
         Collection<(RouteWorker, TimeSpan)> removalEdgesCandidates = new Collection<(RouteWorker, TimeSpan)>();

         foreach (var route in _tour.Edges)
         {
            if (route.Entity.PointTo.Id == _startPOI.Entity.Id)
            {
               continue;
            }

            double tWalkMinutes = (route.Weight() / averageSpeedWalk) / 60.0;
            TimeSpan timeRouteWalk = TimeSpan.FromMinutes(tWalkMinutes);

            if (timeRouteWalk > _timeWalkThreshold)
            {
               var removalEdgeCandidate = (route, timeRouteWalk);
               removalEdgesCandidates.Add(removalEdgeCandidate);
            }
         }
         return removalEdgesCandidates;
      }

      private async Task TryUpdateTour()
      {
         double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed;
         var removalEdgesCandidates = CalculateMaximumEdgesTimeWalk(averageSpeedWalk);

         if (!removalEdgesCandidates.Any())
         {
            return;
         }

         AddPointsNotInTour();
         InterestPointWorker tourPointToRemove = default;
         InterestPointWorker predecessorPoint = default;
         RouteWorker ingoingRoute = default;
         int currentPointScore = int.MaxValue;
         bool tourUpdated = default;

         while (_processingNodes.Any())
         {
            if (tourUpdated)
            {
               break;
            }

            InterestPointWorker candidateNode = _processingNodes.Dequeue();

            foreach (var (edge, tWalk) in removalEdgesCandidates)
            {
               InterestPointWorker currentPointFrom = _tour.TourPoints
                  .Where(point => point.Entity.Id == edge.Entity.PointFrom.Id)
                  .FirstOrDefault();
               if (currentPointFrom is null)
               {
                  throw new NullReferenceException(nameof(currentPointFrom));
               }

               var newEdge = Solver.CityMapGraph
                  .GetEdge(currentPointFrom.Entity.Id, candidateNode.Entity.Id);
               if (newEdge is null)
               {
                  throw new NullReferenceException(nameof(newEdge));
               }

               double tWalkMinutes = (newEdge.Weight() / averageSpeedWalk) / 60.0;
               TimeSpan tEdgeWalk = TimeSpan.FromMinutes(tWalkMinutes);

               if (tEdgeWalk < tWalk)
               {
                  InterestPointWorker currentPointTo = _tour.Nodes
                     .Where(point => point.Entity.Id == edge.Entity.PointTo.Id)
                     .FirstOrDefault();
                  if (currentPointTo is null)
                  {
                     throw new NullReferenceException(nameof(currentPointTo));
                  }

                  int pointToScore = currentPointTo.Entity.Score.Value;
                  if (pointToScore < currentPointScore)
                  {
                     tourPointToRemove = currentPointTo;
                     currentPointScore = pointToScore;
                     predecessorPoint = currentPointFrom;
                     ingoingRoute = edge;
                  }
                  else if (pointToScore == currentPointScore)
                  {
                     tourPointToRemove = (new Random().Next(2) == 0) 
                        ? tourPointToRemove : currentPointTo;
                  }
               }
            }

            if (tourPointToRemove is null)
            {
               continue;
            }

            RouteWorker outgoingEdge = _tour.Edges
               .Where(edge => edge.Entity.PointFrom.Id == tourPointToRemove.Entity.Id)
               .FirstOrDefault();
            if (outgoingEdge is null)
            {
               throw new NullReferenceException(nameof(outgoingEdge));
            }

            InterestPointWorker successorPoint = _tour.Nodes
               .Where(point => point.Entity.Id == outgoingEdge.Entity.PointTo.Id)
               .FirstOrDefault();
            if (successorPoint is null)
            {
               throw new NullReferenceException(nameof(successorPoint));
            }

            UpdateTourInternal(tourPointToRemove, candidateNode, 
               predecessorPoint.Entity.Id, successorPoint.Entity.Id);

            _currentSolution = new TOSolution()
            {
               SolutionGraph = _tour
            };

            tourUpdated = true;
            Solver.EnqueueSolution(_currentSolution);
            await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
            await Solver.AlgorithmTasks[_currentSolution.Id];

            if (!_currentSolution.IsValid)
            {
               UndoUpdateTourInternal(tourPointToRemove, candidateNode.Entity.Id, 
                  predecessorPoint.Entity.Id, successorPoint.Entity.Id);
               tourUpdated = false;
            }
         }

         _isTourUpdated = tourUpdated;
      }

      private void UpdateTourInternal(InterestPointWorker nodeToRemove, InterestPointWorker nodeToAdd, int predecessorNodeKey, int successorNodeKey)
      {
         int nodeKeyToRemove = nodeToRemove.Entity.Id;
         int nodeKeyToAdd = nodeToAdd.Entity.Id;

         _tour.RemoveEdge(predecessorNodeKey, nodeKeyToRemove);
         _tour.RemoveEdge(nodeKeyToRemove, successorNodeKey);
         _tour.RemoveNode(nodeKeyToRemove);

         _tour.AddNode(nodeKeyToAdd, nodeToAdd);
         _tour.AddRouteFromGraph(Solver.CityMapGraph, predecessorNodeKey, nodeKeyToAdd);
         _tour.AddRouteFromGraph(Solver.CityMapGraph, nodeKeyToAdd, successorNodeKey);
      }

      private void UndoUpdateTourInternal(InterestPointWorker nodeToRestore, int nodeKeyToRemove, int predecessorNodeKey, int successorNodeKey)
      {
         int nodeKeyToRestore = nodeToRestore.Entity.Id;

         _tour.RemoveEdge(predecessorNodeKey, nodeKeyToRemove);
         _tour.RemoveEdge(nodeKeyToRemove, successorNodeKey);
         _tour.RemoveNode(nodeKeyToRemove);

         _tour.AddNode(nodeKeyToRestore, nodeToRestore);
         _tour.AddRouteFromGraph(Solver.CityMapGraph, predecessorNodeKey, nodeKeyToRestore);
         _tour.AddRouteFromGraph(Solver.CityMapGraph, nodeKeyToRestore, successorNodeKey);
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         if (Solver.IsMonitoringEnabled)
         {
            SendMessage(MessageCode.CustomAlgStart);
         }

         _processingNodes = new Queue<InterestPointWorker>();
         TOSolution bestSolution = Solver.BestSolution;
         Solver.PreviousStageSolutionCost = bestSolution.Cost;
         _tour = bestSolution.SolutionGraph.DeepCopy();

         //var algorithmParams = Solver.CurrentStage.Flow.AlgorithmParameters;
         //_timeWalkThreshold = algorithmParams[ParameterCodes.HNDTimeWalkThreshold];
         //_tMaxThreshold = algorithmParams[ParameterCodes.HNDTmaxThreshold];

         _timeWalkThreshold = Parameters[ParameterCodes.HNDTimeWalkThreshold];
         _tMaxThreshold = Parameters[ParameterCodes.HNDTmaxThreshold];

         _tMax = Solver.WorkingConfiguration.ArrivalTime
            .Add(Solver.WorkingConfiguration.TourDuration);
         _tMaxThresholdTime = _tMax - _tMaxThreshold;

         _startPOI = _tour.GetStartPoint() ??
            throw new NullReferenceException(nameof(_startPOI));
         _endPOI = _tour.GetEndPoint() ??
            throw new NullReferenceException(nameof(_endPOI));

         AddPointsNotInTour();
         _addedNodesCount = default;
         _isTourUpdated = false;
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

         SendMessage(MessageCode.CustomAlgNodeAdded, point.Entity.Name);
         Solver.EnqueueSolution(_currentSolution);
         await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[_currentSolution.Id];

         if (!_currentSolution.IsValid)
         {
            UndoAdditionPoint(point.Entity.Id, _previousEndPOIKey, _startPOI.Entity.Id);
            _addedNodesCount--;
            _endPOI = _tour.GetEndPoint();
            SendMessage(MessageCode.CustomAlgNodeRemoved, point.Entity.Name);
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
            Task updateTourTask = Task.Run(() => TryUpdateTour());

            try
            {
               updateTourTask.Wait();
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
         }

         if (_currentSolution != null && Solver.BestSolution != null)
         {
            bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(
            _currentSolution.Cost,
            Solver.BestSolution.Cost,
            considerEqualityComparison: true);

            if (isBetterThanCurrentBestSolution)
            {
               Solver.BestSolution = _currentSolution;
            }
         }

         if (_isTourUpdated)
         {
            Task algorithmTask = Task.Run(() => new HybridNearestDistance().Start());

            try
            {
               algorithmTask.Wait();
            }
            catch (AggregateException ae)
            {
               OnError(ae.InnerException);
            }
         }
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         bool isGreaterThanTmaxThreshold = _endPOI.TotalTime > _tMaxThresholdTime;
         bool shouldStop = isGreaterThanTmaxThreshold ||
            !_processingNodes.Any() ||
            Status == AlgorithmStatus.Error;

         return shouldStop;
      }
      #endregion
   }
}