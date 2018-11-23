//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 23/11/2018
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
   internal class HybridDistanceUpdate : HybridDistanceInsertion
   {
      #region Constructors
      internal HybridDistanceUpdate()
         : this(provider: null)
      {
      }
      internal HybridDistanceUpdate(AlgorithmTracker provider)
         : base(provider)
      {
         Type = AlgorithmType.HybridDistanceUpdate;
      }
      #endregion

      #region Internal properties
      internal bool TourUpdated { get; set; }
      #endregion

      #region Private methods
      private IEnumerable<(RouteWorker edge, TimeSpan tWalk)> CalculateMaximumEdgesTimeWalk(double averageSpeedWalk)
      {
         // Set of tuples containing infos: (Route, Travel time)
         Collection<(RouteWorker, TimeSpan)> removalEdgesCandidates = new Collection<(RouteWorker, TimeSpan)>();

         var centralTourRoutes = _tour.Edges
            .Where(edge => edge.Entity.PointFrom.Id != _startPOI.Entity.Id &&
                           edge.Entity.PointTo.Id != _startPOI.Entity.Id);

         foreach (var route in centralTourRoutes)
         {
            double tWalkMinutes = (route.Weight.Invoke() / averageSpeedWalk) / 60.0;
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
               InterestPointWorker currentPointFrom = _tour.Nodes
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

               double tWalkMinutes = (newEdge.Weight.Invoke() / averageSpeedWalk) / 60.0;
               TimeSpan tEdgeWalk = TimeSpan.FromMinutes(tWalkMinutes);

               if (tEdgeWalk < tWalk)
               {
                  InterestPointWorker currentPointTo = _tour.TourPoints
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

            int predecessorPointId = _tour.Edges
               .Where(edge => edge.Entity.PointTo.Id == tourPointToRemove.Entity.Id)
               .Select(edge => edge.Entity.PointFrom.Id)
               .FirstOrDefault();

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
            SendMessage(MessageCode.HNDTourUpdated,
               tourPointToRemove.Entity.Name, candidateNode.Entity.Name);
            Solver.EnqueueSolution(_currentSolution);
            await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
            await Solver.AlgorithmTasks[_currentSolution.Id];

            if (!_currentSolution.IsValid)
            {
               UndoUpdateTourInternal(tourPointToRemove, candidateNode.Entity.Id,
                  predecessorPoint.Entity.Id, successorPoint.Entity.Id);
               tourUpdated = false;
               SendMessage(MessageCode.HNDTourRestored,
                  candidateNode.Entity.Name, tourPointToRemove.Entity.Name);

            }
         }

         TourUpdated = tourUpdated;
      }

      private void UpdateTourInternal(InterestPointWorker nodeToRemove, InterestPointWorker nodeToAdd, 
         int predecessorNodeKey, int successorNodeKey)
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

      private void UndoUpdateTourInternal(InterestPointWorker nodeToRestore, int nodeKeyToRemove,
         int predecessorNodeKey, int successorNodeKey)
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
      }

      internal override Task PerformStep()
      {
         throw new NotImplementedException();
      }

      internal override void OnError(Exception exception)
      {
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
      }

      internal override bool StopConditions()
      {
         return base.StopConditions();
      }
      #endregion
   }
}
