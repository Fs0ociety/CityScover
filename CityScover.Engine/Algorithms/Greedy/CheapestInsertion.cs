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

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// This class implements the Cheapest Insertion algorithm of the Greedy family's algorithms.
   /// </summary>
   internal class CheapestInsertion : GreedyTemplate
   {
      #region Private fields
      private InterestPointWorker _newStartingPoint;
      private InterestPointWorker _endPoint;
      #endregion

      #region Private methods  
      private InterestPointWorker GetCheapestBestNeighbor()
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;
         Collection<InterestPointWorker> candidateNodes = new Collection<InterestPointWorker>();

         foreach (var currentPoint in _tour.Nodes)
         {
            int currentPointId = currentPoint.Entity.Id;
            int currentPointScore = currentPoint.Entity.Score.Value;
            var neighborPoints = _tour.GetAdjacentNodes(currentPointId);

            foreach (var neighborPointId in neighborPoints)
            {
               var processingEdge = _cityMapClone.GetEdge(currentPointId, neighborPointId);
               if (processingEdge is null)
               {
                  return null;
               }

               InterestPointWorker neighborPoint = _cityMapClone[neighborPointId];
               int neighborPointScore = neighborPoint.Entity.Score.Value;
               int currPointToNeighborScore = Math.Abs(currentPointScore - neighborPointScore);

               foreach (var node in _cityMapClone.Nodes)
               {
                  int nodeId = node.Entity.Id;
                  int nodeScore = node.Entity.Score.Value;
                  bool canCompareCosts = !_tour.ContainsNode(nodeId) &&
                     nodeId != neighborPointId &&
                     !node.IsVisited;

                  if (canCompareCosts)
                  {
                     int currPointToNodeScore = Math.Abs(currentPointScore - nodeScore);
                     int nodeToNeighborScore = Math.Abs(nodeScore - neighborPointScore);
                     int deltaScore = currPointToNodeScore + nodeToNeighborScore - currPointToNeighborScore;

                     if (deltaScore > bestScore)
                     {
                        bestScore = deltaScore;
                        candidateNode = node;
                     }
                     else if (deltaScore == 0 || deltaScore == bestScore)
                     {
                        candidateNodes.Add(node);
                     }
                  }
               }

               if (HasToBeSettedCandidateNode(candidateNode, candidateNodes))
               {
                  var nodeIndex = new Random().Next(0, candidateNodes.Count);
                  candidateNode = candidateNodes[nodeIndex];
                  candidateNodes.Clear();
               }
            }
         }

         return candidateNode;
      }

      private InterestPointWorker GetCheapestBestNeighbor(int nodeKey)
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;
         InterestPointWorker processingNode = _cityMapClone[nodeKey];
         Collection<InterestPointWorker> candidateNodes = new Collection<InterestPointWorker>();

         int processingNodeScore = processingNode.Entity.Score.Value;
         var processingNodeNeighbors = _cityMapClone.GetAdjacentNodes(nodeKey);

         foreach (var neighborId in processingNodeNeighbors)
         {
            RouteWorker processingEdge = _cityMapClone.GetEdge(nodeKey, neighborId);
            if (processingEdge is null)
            {
               return default;
            }

            InterestPointWorker neighbor = _cityMapClone[neighborId];
            int neighborScore = neighbor.Entity.Score.Value;
            int pNodeToNeighborScore = Math.Abs(processingNodeScore - neighborScore);

            foreach (var node in _cityMapClone.Nodes)
            {
               int nodeId = node.Entity.Id;
               int nodeScore = node.Entity.Score.Value;

               if (CanCompareCosts(node, nodeId, neighborId))
               {
                  int pNodeToNodeScore = Math.Abs(processingNodeScore - nodeScore);
                  int nodeToNeighborScore = Math.Abs(nodeScore - neighborScore);
                  int deltaScore = pNodeToNodeScore + nodeToNeighborScore - pNodeToNeighborScore;

                  if (deltaScore > bestScore)
                  {
                     bestScore = deltaScore;
                     candidateNode = node;
                  }
                  else if (deltaScore == 0 || deltaScore == bestScore)
                  {
                     candidateNodes.Add(node);
                  }
               }
            }

            if (candidateNode is null && candidateNodes.Any())
            {
               var nodeIndex = new Random().Next(0, candidateNodes.Count);
               candidateNode = candidateNodes[nodeIndex];
            }
            candidateNodes.Clear();
         }

         return candidateNode;
      }

      private bool CanCompareCosts(InterestPointWorker node, int nodeId, int neighborId) =>
         !node.IsVisited && nodeId != neighborId && !_tour.ContainsNode(nodeId);

      private bool HasToBeSettedCandidateNode(InterestPointWorker node, IEnumerable<InterestPointWorker> candidates) =>
         node is null || (node != null && candidates.Any());
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         int startingPointId = _startingPoint.Entity.Id;
         _tour.AddNode(startingPointId, _startingPoint);
         InterestPointWorker bestNeighbor = GetBestNeighbor(_startingPoint);
         if (bestNeighbor is null)
         {
            return;
         }

         bestNeighbor.IsVisited = true;
         int bestNeighborId = bestNeighbor.Entity.Id;
         _tour.AddNode(bestNeighborId, bestNeighbor);
         _tour.AddRouteFromGraph(_cityMapClone, startingPointId, bestNeighborId);
         _tour.AddRouteFromGraph(_cityMapClone, bestNeighborId, startingPointId);
         _newStartingPoint = _startingPoint;
         _endPoint = bestNeighbor;
      }

      internal override async Task PerformStep()
      {
         int processingNodeKey = _processingNodes.Dequeue();
         InterestPointWorker candidateNode = GetCheapestBestNeighbor(processingNodeKey);
         if (candidateNode is null)
         {
            return;
         }

         candidateNode.IsVisited = true;
         _tour.AddNode(candidateNode.Entity.Id, candidateNode);
         _tour.RemoveEdge(_newStartingPoint.Entity.Id, _endPoint.Entity.Id);
         _tour.AddRouteFromGraph(_cityMapClone, _newStartingPoint.Entity.Id, candidateNode.Entity.Id);
         _tour.AddRouteFromGraph(_cityMapClone, candidateNode.Entity.Id, _endPoint.Entity.Id);

         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _tour
         };

         SendMessage(MessageCode.GreedyNodeAdded, candidateNode.Entity.Name);
         _solutions.Add(newSolution);
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[newSolution.Id];

         if (!newSolution.IsValid)
         {
            _tour.RemoveEdge(_newStartingPoint.Entity.Id, candidateNode.Entity.Id);
            _tour.RemoveEdge(candidateNode.Entity.Id, _endPoint.Entity.Id);
            _tour.RemoveNode(candidateNode.Entity.Id);
            _tour.AddRouteFromGraph(_cityMapClone, _newStartingPoint.Entity.Id, _endPoint.Entity.Id);
            SendMessage(MessageCode.GreedyNodeRemoved, candidateNode.Entity.Name);
         }
         else
         {
            _newStartingPoint = candidateNode;
         }

         // Notify observers.
         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(newSolution);
         }
      }

      internal override bool StopConditions()
      {
         return base.StopConditions() || !_processingNodes.Any();
      }
      #endregion
   }
}