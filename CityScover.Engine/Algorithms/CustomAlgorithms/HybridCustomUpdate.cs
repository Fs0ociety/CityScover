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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.CustomAlgorithms
{
   internal class HybridCustomUpdate : HybridCustomInsertion
   {
      private double _averageSpeedWalk;
      private TOSolution _currentSolution;
      private IEnumerable<(RouteWorker, TimeSpan)> _candidateEdges;

      #region Constructors
      internal HybridCustomUpdate()
         : this(provider: null)
      {
      }
      internal HybridCustomUpdate(AlgorithmTracker provider)
         : base(provider)
      {
         Type = AlgorithmType.HybridCustomUpdate;
      }
      #endregion

      #region Internal properties
      internal bool TourUpdated { get; set; }
      internal TOSolution CurrentSolution => _currentSolution;
      #endregion

      #region Private methods
      private IEnumerable<(RouteWorker edge, TimeSpan tWalk)> CalculateMaxEdgesTimeWalk()
      {
         // Set of tuples containing infos: (Route, Travel time)
         Collection<(RouteWorker, TimeSpan)> removalEdgesCandidates = new Collection<(RouteWorker, TimeSpan)>();

         var centralTourRoutes = _tour.Edges
            .Where(edge => edge.Entity.PointFrom.Id != _startPOI.Entity.Id &&
                           edge.Entity.PointTo.Id != _startPOI.Entity.Id);

         foreach (var route in centralTourRoutes)
         {
            double tWalkMinutes = (route.Weight.Invoke() / _averageSpeedWalk) / 60.0;
            TimeSpan timeRouteWalk = TimeSpan.FromMinutes(tWalkMinutes);

            if (timeRouteWalk > _timeWalkThreshold)
            {
               var removalEdgeCandidate = (route, timeRouteWalk);
               removalEdgesCandidates.Add(removalEdgeCandidate);
            }
         }

         return removalEdgesCandidates;
      }

      private int FindPointToRemove(int candidateNodeKey)
      {
         int nodeKeyToRemove = default;
         int currentPointScore = int.MaxValue;

         foreach (var (edge, tWalk) in _candidateEdges)
         {
            var newEdge = Solver.CityMapGraph
               .GetEdge(edge.Entity.PointFrom.Id, candidateNodeKey);
            if (newEdge is null)
            {
               throw new NullReferenceException(nameof(newEdge));
            }

            double tWalkMinutes = (newEdge.Weight.Invoke() / _averageSpeedWalk) / 60.0;
            TimeSpan tEdgeWalk = TimeSpan.FromMinutes(tWalkMinutes);

            if (tEdgeWalk < tWalk)
            {
               int currentPointToId = _tour.Edges
                  .Where(e => e.Entity.PointFrom.Id == edge.Entity.PointTo.Id)
                  .Select(e => e.Entity.PointTo.Id)
                  .FirstOrDefault();

               if (!_tour.ContainsNode(currentPointToId))
               {
                  throw new NullReferenceException(nameof(currentPointToId));
               }

               InterestPointWorker currentPointTo = _tour[currentPointToId];
               int pointToScore = currentPointTo.Entity.Score.Value;
               if (pointToScore < currentPointScore)
               {
                  nodeKeyToRemove = currentPointTo.Entity.Id;
                  currentPointScore = pointToScore;
               }
               else if (pointToScore == currentPointScore)
               {
                  // Random choose
                  nodeKeyToRemove = (new Random().Next(2) == 0)
                     ? nodeKeyToRemove : currentPointTo.Entity.Id;
               }
            }
         }

         return nodeKeyToRemove;
      }

      private (int, int) GetBorderPoints(int nodeKeyToRemove)
      {
         int predecessorNodeKey = _tour.Edges
            .Where(edge => edge.Entity.PointTo.Id == nodeKeyToRemove)
            .Select(edge => edge.Entity.PointFrom.Id)
            .FirstOrDefault();

         int successorNodeKey = _tour.Edges
            .Where(edge => edge.Entity.PointFrom.Id == nodeKeyToRemove)
            .Select(edge => edge.Entity.PointTo.Id)
            .FirstOrDefault();

         return (predecessorNodeKey, successorNodeKey);
      }

      private void UpdateTour(InterestPointWorker nodeToAdd, int nodeKeyToRemove, int predecessorNodeKey, int successorNodeKey)
      {
         int nodeKeyToAdd = nodeToAdd.Entity.Id;

         _tour.RemoveEdge(predecessorNodeKey, nodeKeyToRemove);
         _tour.RemoveEdge(nodeKeyToRemove, successorNodeKey);
         _tour.RemoveNode(nodeKeyToRemove);

         _tour.AddNode(nodeKeyToAdd, nodeToAdd);
         _tour.AddRouteFromGraph(Solver.CityMapGraph, predecessorNodeKey, nodeKeyToAdd);
         _tour.AddRouteFromGraph(Solver.CityMapGraph, nodeKeyToAdd, successorNodeKey);
      }

      private void UndoUpdate(InterestPointWorker nodeToRestore, int nodeKeyToRemove, int predecessorNodeKey, int successorNodeKey)
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
         _averageSpeedWalk = Solver.WorkingConfiguration.WalkingSpeed;
         _candidateEdges = new Collection<(RouteWorker, TimeSpan)>();
         _candidateEdges = CalculateMaxEdgesTimeWalk();
      }

      internal override async Task PerformStep()
      {
         InterestPointWorker candidateNode = _processingNodes.Dequeue();
         int nodeKeyToRemove = FindPointToRemove(candidateNode.Entity.Id);
         var (predecessorNodeKey, successorNodeKey) = GetBorderPoints(nodeKeyToRemove);
         InterestPointWorker nodeToRemove = _tour.TourPoints
            .Where(point => point.Entity.Id == nodeKeyToRemove)
            .FirstOrDefault();
         UpdateTour(candidateNode, nodeKeyToRemove, predecessorNodeKey, successorNodeKey);

         _currentSolution = new TOSolution()
         {
            SolutionGraph = _tour.DeepCopy()
         };

         TourUpdated = true;
         SendMessage(MessageCode.HDUTourUpdated, nodeToRemove.Entity.Name, candidateNode.Entity.Name);
         Solver.EnqueueSolution(_currentSolution);
         _solutionsHistory.Add(_currentSolution);
         await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[_currentSolution.Id];

         if (!_currentSolution.IsValid)
         {
            UndoUpdate(nodeToRemove, candidateNode.Entity.Id, predecessorNodeKey, successorNodeKey);
            SendMessage(MessageCode.HDUTourRestored, candidateNode.Entity.Name, nodeToRemove.Entity.Name);
            TourUpdated = false;
         }

         // Notify observers.
         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(_currentSolution);
         }
      }

      internal override void OnTerminating()
      {
      }

      internal override bool StopConditions()
      {
         bool shouldStop = !_candidateEdges.Any() || TourUpdated || base.StopConditions();
         return shouldStop;
      }
      #endregion
   }
}
