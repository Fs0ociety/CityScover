//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 20/12/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using System;
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

      #region Constructors
      internal CheapestInsertion() : this(null)
      {
      }

      internal CheapestInsertion(AlgorithmTracker provider) : base(provider)
         => Type = AlgorithmType.CheapestInsertion;
      #endregion

      #region Private methods
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

      private (int fromNodeKey, int toNodeKey, int kCandidate) GetCheapestBestNeighbor()
      {
         int fromNodeKey = default;
         int toNodeKey = default;
         int kCandidate = default;
         int smallestDelta = int.MaxValue;

         foreach (var edge in Tour.Edges)
         {
            foreach (var kNode in NodesQueue)
            {
               var edgeik = CityMapClone.GetEdge(edge.Entity.PointFrom.Id, kNode);
               var edgekj = CityMapClone.GetEdge(kNode, edge.Entity.PointTo.Id);
               var edgeij = CityMapClone.GetEdge(edge.Entity.PointFrom.Id, edge.Entity.PointTo.Id);

               if (edgeik is null || edgekj is null || edgeij is null)
               {
                  return (default, default, default);
               }

               int deltaCost;
               var cik = edgeik.Weight.Invoke();
               var ckj = edgekj.Weight.Invoke();
               var cij = edgeij.Weight.Invoke();

               try
               {
                  deltaCost = Convert.ToInt32(cik + ckj - cij);
               }
               catch
               {
                  return (default, default, default);
               }

               if (deltaCost < smallestDelta)
               {
                  smallestDelta = deltaCost;
                  fromNodeKey = edge.Entity.PointFrom.Id;
                  toNodeKey = edge.Entity.PointTo.Id;
                  kCandidate = kNode;
               }
               else if (deltaCost == smallestDelta)
               {
                  var kNodePoi = CityMapClone[kNode];
                  var kCandidatePoi = CityMapClone[kCandidate];
                  var kNodePoiScore = kNodePoi.Entity.Score.Value;
                  var kCandidatePoiScore = kCandidatePoi.Entity.Score.Value;
                  var areScoreEquals = kNodePoiScore == kCandidatePoiScore;

                  if (areScoreEquals)
                  {
                     CityMapGraph.SetRandomCandidateId(kCandidatePoi, kNodePoi, out kCandidate);
                  }
                  else
                  {
                     kCandidate = kCandidatePoiScore > kNodePoiScore 
                        ? kCandidatePoi.Entity.Id 
                        : kNodePoi.Entity.Id;
                  }
               }
            }
         }

         return (fromNodeKey, toNodeKey, kCandidate);
      }

      private void AddPointsNotInTour()
      {
         #region Method 1: LINQ with Except
         var tourNodesId = Tour.Nodes.Select(node => node.Entity.Id);
         var cityMapNodes = CityMapClone.Nodes.Select(node => node.Entity.Id);
         var nodesNotInTour = cityMapNodes.Except(tourNodesId);

         NodesQueue.Clear();
         nodesNotInTour.ToList().ForEach(node => NodesQueue.Enqueue(node));
         #endregion

         #region Method 2: LINQ with Where & Select
         //NodesQueue.Clear();
         //CityMapClone.Nodes
         //   .Where(node => !Tour.ContainsNode(node.Entity.Id))
         //   .Select(node => node.Entity.Id)
         //   .ToList()
         //   .ForEach(node => NodesQueue.Enqueue(node));
         #endregion
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         int startingPointId = StartingPoint.Entity.Id;
         InterestPointWorker bestNeighbor = GetBestNeighbor(StartingPoint);
         if (bestNeighbor is null)
         {
            return;
         }

         bestNeighbor.IsVisited = true;
         int bestNeighborId = bestNeighbor.Entity.Id;
         Tour.AddNode(startingPointId, StartingPoint);
         Tour.AddNode(bestNeighborId, bestNeighbor);
         Tour.AddRouteFromGraph(CityMapClone, startingPointId, bestNeighborId);
         Tour.AddRouteFromGraph(CityMapClone, bestNeighborId, startingPointId);
         AddPointsNotInTour();
         _newStartingPoint = StartingPoint;
         _endPoint = bestNeighbor;
      }

      protected override async Task PerformStep()
      {
         int processingNodeKey = NodesQueue.Dequeue();
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

         ToSolution newSolution = new ToSolution()
         {
            SolutionGraph = Tour
         };

         SendMessage(MessageCode.GreedyNodeAdded, candidateNode.Entity.Name, newSolution.Id);
         SolutionsHistory.Add(newSolution);
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(Utils.ValidationDelay).ConfigureAwait(continueOnCapturedContext: false);
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