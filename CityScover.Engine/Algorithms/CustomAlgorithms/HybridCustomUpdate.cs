//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 16/12/2018
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
   internal class HybridCustomUpdate : HybridCustomInsertion
   {
      private double _averageSpeedWalk;
      private ToSolution _currentSolution;
      private ICollection<(RouteWorker, TimeSpan)> _candidateEdges;

      #region Constructors
      internal HybridCustomUpdate() : this(null)
      {
      }
      internal HybridCustomUpdate(AlgorithmTracker provider) : base(provider)
         => Type = AlgorithmType.HybridCustomUpdate;
      #endregion

      #region Internal properties
      internal bool TourUpdated { get; private set; }
      internal ToSolution CurrentSolution => _currentSolution;
      #endregion

      #region Private methods
      private ICollection<(RouteWorker edge, TimeSpan tWalk)> CalculateMaxEdgesTimeWalk()
      {
         // Set of tuples containing infos: (Route, Travel time)
         Collection<(RouteWorker, TimeSpan)> removalEdgesCandidates = new Collection<(RouteWorker, TimeSpan)>();

         var centralTourRoutes = Tour.Edges
            .Where(edge => edge.Entity.PointFrom.Id != StartPoi.Entity.Id &&
                           edge.Entity.PointTo.Id != StartPoi.Entity.Id);

         foreach (var route in centralTourRoutes)
         {
            double tWalkMinutes = (route.Weight.Invoke() / _averageSpeedWalk) / 60.0;
            TimeSpan timeRouteWalk = TimeSpan.FromMinutes(tWalkMinutes);

            if (timeRouteWalk > TimeWalkThreshold)
            {
               var removalEdgeCandidate = (route, timeRouteWalk);
               removalEdgesCandidates.Add(removalEdgeCandidate);
            }
         }

         return removalEdgesCandidates;
      }

      private InterestPointWorker FindPointToRemove(int candidateNodeKey)
      {
         InterestPointWorker nodeKeyToRemove = default;
         int currentPointScore = int.MaxValue;

         foreach (var (edge, tWalk) in _candidateEdges.ToList())
         {
            if (nodeKeyToRemove != default)
            {
               break;
            }

            int currentPointToId = edge.Entity.PointTo.Id;
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
               if (!Tour.ContainsNode(currentPointToId))
               {
                  throw new NullReferenceException(nameof(currentPointToId));
               }

               var currentPointTo = Tour[currentPointToId];
               int pointToScore = currentPointTo.Entity.Score.Value;
               if (pointToScore < currentPointScore)
               {
                  nodeKeyToRemove = currentPointTo;
                  currentPointScore = pointToScore;
                  _candidateEdges.Remove((edge, tWalk));
               }
               else if (pointToScore == currentPointScore)
               {
                  // Random choose
                  nodeKeyToRemove = (new Random().Next(2) == 0)
                     ? nodeKeyToRemove : currentPointTo;
               }
            }
         }

         return nodeKeyToRemove;
      }

      private (int, int) GetBorderPoints(int nodeKeyToRemove)
      {
         int predecessorNodeKey = Tour.Edges
            .Where(edge => edge.Entity.PointTo.Id == nodeKeyToRemove)
            .Select(edge => edge.Entity.PointFrom.Id)
            .FirstOrDefault();

         int successorNodeKey = Tour.Edges
            .Where(edge => edge.Entity.PointFrom.Id == nodeKeyToRemove)
            .Select(edge => edge.Entity.PointTo.Id)
            .FirstOrDefault();

         return (predecessorNodeKey, successorNodeKey);
      }

      private void UpdateTour(InterestPointWorker nodeToAdd, int nodeKeyToRemove, int predecessorNodeKey, int successorNodeKey)
      {
         int nodeKeyToAdd = nodeToAdd.Entity.Id;

         Tour.RemoveEdge(predecessorNodeKey, nodeKeyToRemove);
         Tour.RemoveEdge(nodeKeyToRemove, successorNodeKey);
         Tour.RemoveNode(nodeKeyToRemove);

         Tour.AddNode(nodeKeyToAdd, nodeToAdd);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, predecessorNodeKey, nodeKeyToAdd);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, nodeKeyToAdd, successorNodeKey);
      }

      private void UndoUpdate(InterestPointWorker nodeToRestore, int nodeKeyToRemove, int predecessorNodeKey, int successorNodeKey)
      {
         int nodeKeyToRestore = nodeToRestore.Entity.Id;

         Tour.RemoveEdge(predecessorNodeKey, nodeKeyToRemove);
         Tour.RemoveEdge(nodeKeyToRemove, successorNodeKey);
         Tour.RemoveNode(nodeKeyToRemove);

         Tour.AddNode(nodeKeyToRestore, nodeToRestore);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, predecessorNodeKey, nodeKeyToRestore);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, nodeKeyToRestore, successorNodeKey);
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

         if (Solver.IsMonitoringEnabled)
         {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            SendMessage(MessageCode.HybridDistanceUpdateStart);
            Console.ForegroundColor = ConsoleColor.Gray;
         }

         TourUpdated = default;
         _averageSpeedWalk = Solver.WorkingConfiguration.WalkingSpeed;
         _candidateEdges = new Collection<(RouteWorker, TimeSpan)>();
         _candidateEdges = CalculateMaxEdgesTimeWalk();
      }

      protected override async Task PerformStep()
      {
         var candidateNode = ProcessingNodes.Dequeue();
         var nodeToRemove = FindPointToRemove(candidateNode.Entity.Id);
         if (nodeToRemove == default)
         {
            return;
         }

         int nodeKeyToRemove = nodeToRemove.Entity.Id;
         var (predecessorNodeKey, successorNodeKey) = GetBorderPoints(nodeKeyToRemove);
         UpdateTour(candidateNode, nodeKeyToRemove, predecessorNodeKey, successorNodeKey);

         _currentSolution = new ToSolution()
         {
            SolutionGraph = Tour.DeepCopy()
         };

         TourUpdated = true;
         SendMessage(MessageCode.HybridDistanceUpdateTourUpdated, nodeToRemove.Entity.Name, candidateNode.Entity.Name);
         Solver.EnqueueSolution(_currentSolution);
         SolutionsHistory.Add(_currentSolution);

         await Task.Delay(Utils.ValidationDelay).ConfigureAwait(false);
         await Solver.AlgorithmTasks[_currentSolution.Id];

         if (!_currentSolution.IsValid)
         {
            UndoUpdate(nodeToRemove, candidateNode.Entity.Id, predecessorNodeKey, successorNodeKey);
            SendMessage(MessageCode.HybridDistanceUpdateTourRestored, candidateNode.Entity.Name, nodeToRemove.Entity.Name);
            TourUpdated = false;
         }
         else
         {
            _candidateEdges.Clear();
            _candidateEdges = CalculateMaxEdgesTimeWalk();
         }

         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(_currentSolution);
         }
      }

      internal override void OnTerminating()
      {
         if (_currentSolution != null)
         {
            SendMessage(MessageCode.HybridDistanceUpdateStopWithSolution,
               _currentSolution.Id, _currentSolution.Cost);

            _currentSolution = SolutionsHistory
               .Where(solution => solution.IsValid)
               .MaxBy(solution => solution.Cost);

            var isBetterThanCurrentBestSolution = Solver.Problem
               .CompareSolutionsCost(_currentSolution.Cost, Solver.BestSolution.Cost, true);

            if (isBetterThanCurrentBestSolution)
            {
               Solver.BestSolution = _currentSolution;
            }
         }
         else
         {
            SendMessage(MessageCode.HybridDistanceUpdateStopWithoutSolution);
         }
      }

      internal override bool StopConditions()
      {
         bool shouldStop = !_candidateEdges.Any() || base.StopConditions();
         return shouldStop;
      }
      #endregion
   }
}
