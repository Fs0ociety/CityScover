//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 12/01/2019
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
      private TimeSpan _timeWalkThreshold;
      private ICollection<(RouteWorker, TimeSpan)> _candidateEdges;
      private ICollection<(int nodeKeyRemoved, int nodeKeyAdded)> _replacedPoints;

      #region Constructors
      internal HybridCustomUpdate() : this(null)
      {
      }
      internal HybridCustomUpdate(AlgorithmTracker provider) : base(provider)
         => Type = AlgorithmType.HybridCustomUpdate;
      #endregion

      #region Internal properties
      internal bool TourUpdated { get; private set; }
      internal ToSolution CurrentSolution { get; private set; }
      internal bool CanContinueToRelaxConstraints { get; set; } = true;
      #endregion

      #region Private methods
      private ICollection<(RouteWorker edge, TimeSpan tWalk)> CalculateMaxEdgesTimeWalk()
      {
         // Set of tuples containing infos: (Route, Travel time)
         var removalEdgesCandidates = new Collection<(RouteWorker, TimeSpan)>();

         var centralTourRoutes = Tour.Edges
            .Where(edge => edge.Entity.PointFrom.Id != StartPoi.Entity.Id &&
                           edge.Entity.PointTo.Id != StartPoi.Entity.Id);

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

      private InterestPointWorker GetPointToRemove(int candidateNodeKey)
      {
         InterestPointWorker nodeKeyToRemove = default;
         int currentPointScore = int.MaxValue;

         foreach (var (edge, tWalk) in _candidateEdges)
         {
            int currentPointToId = edge.Entity.PointTo.Id;
            var newEdge = Solver.CityMapGraph
               .GetEdge(edge.Entity.PointFrom.Id, candidateNodeKey);
            if (newEdge is null)
            {
               throw new NullReferenceException(nameof(newEdge));
            }

            double tWalkMinutes = (newEdge.Weight.Invoke() / _averageSpeedWalk) / 60.0;
            TimeSpan tEdgeWalk = TimeSpan.FromMinutes(tWalkMinutes);

            if (tEdgeWalk >= tWalk)
            {
               continue;
            }

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
            }
            else if (pointToScore == currentPointScore)
            {
               nodeKeyToRemove = new Random().Next(2) == 0
                  ? nodeKeyToRemove : currentPointTo;
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

      private void UpdateTour(InterestPointWorker nodeToAdd,
         int nodeKeyToRemove, int predecessorNodeKey, int successorNodeKey)
      {
         int nodeKeyToAdd = nodeToAdd.Entity.Id;

         Tour.RemoveEdge(predecessorNodeKey, nodeKeyToRemove);
         Tour.RemoveEdge(nodeKeyToRemove, successorNodeKey);
         Tour.RemoveNode(nodeKeyToRemove);

         Tour.AddNode(nodeKeyToAdd, nodeToAdd);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, predecessorNodeKey, nodeKeyToAdd);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, nodeKeyToAdd, successorNodeKey);
      }

      private void UndoUpdate(InterestPointWorker nodeToRestore,
         int nodeKeyToRemove, int predecessorNodeKey, int successorNodeKey)
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
         if (!Parameters.ContainsKey(ParameterCodes.HcuTimeWalkThreshold))
         {
            throw new KeyNotFoundException(nameof(ParameterCodes.HcuTimeWalkThreshold));
         }

         base.OnInitializing();
         _timeWalkThreshold = Parameters[ParameterCodes.HcuTimeWalkThreshold];
         _averageSpeedWalk = Solver.WorkingConfiguration.WalkingSpeed;
         _candidateEdges = new Collection<(RouteWorker, TimeSpan)>();
         _replacedPoints = new Collection<(int, int)>();
         _candidateEdges = CalculateMaxEdgesTimeWalk();
         Solver.ConstraintsToRelax.Remove(Utils.TMaxConstraint);

         if (!CanContinueToRelaxConstraints)
         {
            Solver.ConstraintsToRelax.Clear();
         }
         if (Solver.IsMonitoringEnabled)
         {
            Console.ForegroundColor = ConsoleColor.Magenta;
            SendMessage(MessageCode.HybridCustomUpdateStart);
            Console.ForegroundColor = ConsoleColor.Gray;
         }
      }

      protected override async Task PerformStep()
      {
         var candidateNode = ProcessingNodes.Dequeue();
         var nodeToRemove = GetPointToRemove(candidateNode.Entity.Id);
         if (nodeToRemove == default)
         {
            return;
         }

         int nodeKeyToRemove = nodeToRemove.Entity.Id;
         var (predecessorNodeKey, successorNodeKey) = GetBorderPoints(nodeKeyToRemove);
         UpdateTour(candidateNode, nodeKeyToRemove, predecessorNodeKey, successorNodeKey);

         CurrentSolution = new ToSolution()
         {
            SolutionGraph = Tour.DeepCopy()
         };

         SendMessage(MessageCode.HybridCustomUpdateTourUpdated, nodeToRemove.Entity.Name, candidateNode.Entity.Name);
         Solver.EnqueueSolution(CurrentSolution);
         SolutionsHistory.Add(CurrentSolution);

         await Task.Delay(Utils.ValidationDelay).ConfigureAwait(false);
         await Solver.AlgorithmTasks[CurrentSolution.Id].ConfigureAwait(false);

         if (!CurrentSolution.IsValid)
         {
            UndoUpdate(nodeToRemove, candidateNode.Entity.Id, predecessorNodeKey, successorNodeKey);
            SendMessage(MessageCode.HybridCustomUpdateTourRestored, candidateNode.Entity.Name, nodeToRemove.Entity.Name);
         }
         else
         {
            var (nodeKeyRemoved, nodeKeyAdded) = ValueTuple.Create(nodeKeyToRemove, candidateNode.Entity.Id);
            _replacedPoints.Add((nodeKeyRemoved, nodeKeyAdded));
            _candidateEdges.Clear();
            _candidateEdges = CalculateMaxEdgesTimeWalk();
         }

         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(CurrentSolution);
         }
      }

      internal override void OnTerminating()
      {
         if (CurrentSolution is null)
         {
            Console.ForegroundColor = ConsoleColor.Magenta;
            SendMessage(MessageCode.HybridCustomUpdateStopWithoutSolution);
            Console.ForegroundColor = ConsoleColor.Gray;

            return;
         }

         Console.ForegroundColor = ConsoleColor.Magenta;
         SendMessage(MessageCode.HybridCustomUpdateStopWithSolution,
            CurrentSolution.Id, CurrentSolution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;

         var validSolutions = SolutionsHistory.Where(solution => solution.IsValid);
         TourUpdated = validSolutions.Any();

         if (!TourUpdated)
         {
            Console.ForegroundColor = ConsoleColor.Magenta;
            SendMessage(MessageCode.HybridCustomUpdateFailed);
            Console.ForegroundColor = ConsoleColor.Gray;

            return;
         }

         CurrentSolution = validSolutions.MaxBy(solution => solution.Cost);
         var hcuTourDistance = CurrentSolution.SolutionGraph.GetTotalDistance();
         var solverTourDistance = Solver.BestSolution.SolutionGraph.GetTotalDistance();

         bool canUpdate = hcuTourDistance < solverTourDistance || 
                          hcuTourDistance.IsApproximatelyEqualTo(solverTourDistance);
         if (!canUpdate)
         {
            Console.ForegroundColor = ConsoleColor.Magenta;
            SendMessage(MessageCode.HybridCustomUpdateSolutionNotUpdated,
               CurrentSolution.Id, CurrentSolution.Cost, hcuTourDistance * 0.001);
            Console.ForegroundColor = ConsoleColor.Gray;

            return;
         }

         // TODO
         // Correggere logica di confronto usando la StartingSolution e non Solver.BestSolution
         // e sistemare le stampe di conseguenza.
         if (CurrentSolution.Cost > Solver.BestSolution.Cost)
         {
            UpdateSolver(CurrentSolution, hcuTourDistance, solverTourDistance,
               MessageCode.HybridCustomUpdateBestFinalTour, ConsoleColor.Magenta);
         }
         else if (CurrentSolution.Cost < Solver.BestSolution.Cost)
         {
            UpdateSolver(CurrentSolution, hcuTourDistance, solverTourDistance,
               MessageCode.HybridCustomUpdateWorseFinalTour, ConsoleColor.Magenta);
         }
         else
         {
            UpdateSolver(CurrentSolution, hcuTourDistance, solverTourDistance,
               MessageCode.HybridCustomUpdateCostUnchanged, ConsoleColor.Magenta);
         }
      }

      internal override void OnTerminated()
      {
         foreach (var (nodeKeyRemoved, nodeKeyAdded) in _replacedPoints)
         {
            SendMessage(MessageCode.HybridCustomUpdatePointsReplaced, nodeKeyRemoved, nodeKeyAdded);
         }

         if (TourUpdated)
         {
            SendMessage(ToSolution.SolutionCollectionToString(SolutionsHistory));
         }

         SolutionsHistory.Clear();
         _replacedPoints.Clear();
         _candidateEdges.Clear();
         _replacedPoints = null;
         _candidateEdges = null;
         SolutionsHistory = null;
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return !_candidateEdges.Any() || base.StopConditions();
      }
      #endregion
   }
}
