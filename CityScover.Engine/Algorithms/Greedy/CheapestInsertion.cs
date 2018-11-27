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

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// This class implements the Cheapest Insertion algorithm of the Greedy family's algorithms.
   /// </summary>
   internal class CheapestInsertion : GreedyTemplate
   {
      private InterestPointWorker _newStartingPoint;
      private InterestPointWorker _endPoint;

      #region Private methods  
      //private InterestPointWorker GetCheapestBestNeighbor()
      //{
      //   int bestScore = default;
      //   InterestPointWorker candidateNode = default;
      //   Collection<InterestPointWorker> candidateNodes = new Collection<InterestPointWorker>();

      //   foreach (var currentPoint in Tour.Nodes)
      //   {
      //      int currentPointId = currentPoint.Entity.Id;
      //      int currentPointScore = currentPoint.Entity.Score.Value;
      //      var neighborPoints = Tour.GetAdjacentNodes(currentPointId);

      //      foreach (var neighborPointId in neighborPoints)
      //      {
      //         var processingEdge = CityMapClone.GetEdge(currentPointId, neighborPointId);
      //         if (processingEdge is null)
      //         {
      //            return null;
      //         }

      //         InterestPointWorker neighborPoint = CityMapClone[neighborPointId];
      //         int neighborPointScore = neighborPoint.Entity.Score.Value;
      //         int currPointToNeighborScore = Math.Abs(currentPointScore - neighborPointScore);

      //         foreach (var node in CityMapClone.Nodes)
      //         {
      //            int nodeId = node.Entity.Id;
      //            int nodeScore = node.Entity.Score.Value;
      //            bool canCompareCosts = !Tour.ContainsNode(nodeId) &&
      //               nodeId != neighborPointId &&
      //               !node.IsVisited;

      //            if (canCompareCosts)
      //            {
      //               int currPointToNodeScore = Math.Abs(currentPointScore - nodeScore);
      //               int nodeToNeighborScore = Math.Abs(nodeScore - neighborPointScore);
      //               int deltaScore = currPointToNodeScore + nodeToNeighborScore - currPointToNeighborScore;

      //               if (deltaScore > bestScore)
      //               {
      //                  bestScore = deltaScore;
      //                  candidateNode = node;
      //               }
      //               else if (deltaScore == 0 || deltaScore == bestScore)
      //               {
      //                  candidateNodes.Add(node);
      //               }
      //            }
      //         }

      //         if (HasToBeSetCandidateNode(candidateNode, candidateNodes))
      //         {
      //            var nodeIndex = new Random().Next(0, candidateNodes.Count);
      //            candidateNode = candidateNodes[nodeIndex];
      //            candidateNodes.Clear();
      //         }
      //      }
      //   }
   
      //   return candidateNode;
      //}

      private InterestPointWorker GetCheapestBestNeighbor(int nodeKey)
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;
         InterestPointWorker processingNode = CityMapClone[nodeKey];
         Collection<InterestPointWorker> candidateNodes = new Collection<InterestPointWorker>();

         int processingNodeScore = processingNode.Entity.Score.Value;
         var processingNodeNeighbors = CityMapClone.GetAdjacentNodes(nodeKey);

         foreach (var neighborId in processingNodeNeighbors)
         {
            RouteWorker processingEdge = CityMapClone.GetEdge(nodeKey, neighborId);
            if (processingEdge is null)
            {
               return default;
            }

            InterestPointWorker neighbor = CityMapClone[neighborId];
            int neighborScore = neighbor.Entity.Score.Value;
            int pNodeToNeighborScore = Math.Abs(processingNodeScore - neighborScore);

            foreach (var node in CityMapClone.Nodes)
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
         !node.IsVisited && nodeId != neighborId && !Tour.ContainsNode(nodeId);

      private bool HasToBeSetCandidateNode(InterestPointWorker node, IEnumerable<InterestPointWorker> candidates) =>
         node is null || candidates.Any();
      //node is null || (node != null && candidates.Any());
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         int startingPointId = StartingPoint.Entity.Id;
         Tour.AddNode(startingPointId, StartingPoint);
         InterestPointWorker bestNeighbor = GetBestNeighbor(StartingPoint);
         if (bestNeighbor is null)
         {
            return;
         }

         bestNeighbor.IsVisited = true;
         int bestNeighborId = bestNeighbor.Entity.Id;
         Tour.AddNode(bestNeighborId, bestNeighbor);
         Tour.AddRouteFromGraph(CityMapClone, startingPointId, bestNeighborId);
         Tour.AddRouteFromGraph(CityMapClone, bestNeighborId, startingPointId);
         _newStartingPoint = StartingPoint;
         _endPoint = bestNeighbor;
      }

      protected override async Task PerformStep()
      {
         int processingNodeKey = ProcessingNodes.Dequeue();
         InterestPointWorker candidateNode = GetCheapestBestNeighbor(processingNodeKey);
         if (candidateNode is null)
         {
            return;
         }

         candidateNode.IsVisited = true;
         Tour.AddNode(candidateNode.Entity.Id, candidateNode);
         Tour.RemoveEdge(_newStartingPoint.Entity.Id, _endPoint.Entity.Id);
         Tour.AddRouteFromGraph(CityMapClone, _newStartingPoint.Entity.Id, candidateNode.Entity.Id);
         Tour.AddRouteFromGraph(CityMapClone, candidateNode.Entity.Id, _endPoint.Entity.Id);

         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = Tour
         };

         SendMessage(MessageCode.GreedyNodeAdded, candidateNode.Entity.Name);
         SolutionsHistory.Add(newSolution);
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[newSolution.Id];

         if (!newSolution.IsValid)
         {
            Tour.RemoveEdge(_newStartingPoint.Entity.Id, candidateNode.Entity.Id);
            Tour.RemoveEdge(candidateNode.Entity.Id, _endPoint.Entity.Id);
            Tour.RemoveNode(candidateNode.Entity.Id);
            Tour.AddRouteFromGraph(CityMapClone, _newStartingPoint.Entity.Id, _endPoint.Entity.Id);
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
      #endregion
   }
}