//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/12/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// This class implements the Cheapest Insertion algorithm of the Greedy family's algorithms.
   /// </summary>
   internal class CheapestInsertion : GreedyTemplate
   {
      #region Constructors
      internal CheapestInsertion() : this(null)
      {
      }

      internal CheapestInsertion(AlgorithmTracker provider) : base(provider)
         => Type = AlgorithmType.CheapestInsertion;
      #endregion

      #region Private methods
      private void AddPointsNotInTour()
      {
         #region Method 1: LINQ with Except
         NodesQueue.Clear();

         CityMapClone.Nodes
            .Except(Tour.Nodes)
            .Where(node => !node.IsVisited)
            .ToList()
            .ForEach(node => NodesQueue.Enqueue(node.Entity.Id));
         #endregion

         #region Method 2: LINQ with Where & Select
         //NodesQueue.Clear();

         //CityMapClone.Nodes
         //   .Where(node => !node.IsVisited && !Tour.ContainsNode(node.Entity.Id))
         //   .Select(node => node.Entity.Id)
         //   .ToList()
         //   .ForEach(node => NodesQueue.Enqueue(node));
         #endregion
      }

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
                  if (kNode == kCandidate)
                  {
                     continue;
                  }
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
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         int startingPointId = StartingPoint.Entity.Id;
         var bestNeighbor = GetBestNeighbor(StartingPoint);
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
      }

      protected override async Task PerformStep()
      {
         var (iNode, jNode, kNode) = GetCheapestBestNeighbor();
         if ((iNode, jNode, kNode) == default)
         {
            return;
         }

         var candidateNode = CityMapClone[kNode];
         Tour.AddNode(kNode, candidateNode);
         Tour.RemoveEdge(iNode, jNode);
         Tour.AddRouteFromGraph(CityMapClone, iNode, kNode);
         Tour.AddRouteFromGraph(CityMapClone, kNode, jNode);
         candidateNode.IsVisited = true;

         var newSolution = new ToSolution()
         {
            SolutionGraph = Tour.DeepCopy()
         };

         SendMessage(MessageCode.GreedyNodeAdded, candidateNode.Entity.Name, newSolution.Id);
         SolutionsHistory.Add(newSolution);
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(Utils.ValidationDelay).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[newSolution.Id].ConfigureAwait(false);

         if (!newSolution.IsValid)
         {
            Tour.RemoveEdge(iNode, kNode);
            Tour.RemoveEdge(kNode, jNode);
            Tour.RemoveNode(kNode);
            Tour.AddRouteFromGraph(CityMapClone, iNode, jNode);
            SendMessage(MessageCode.GreedyNodeRemoved, candidateNode.Entity.Name);
         }

         AddPointsNotInTour();

         // Notify observers.
         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(newSolution);
         }
      }
      #endregion
   }
}