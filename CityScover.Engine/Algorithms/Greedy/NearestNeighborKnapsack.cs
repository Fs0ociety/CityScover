//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 14/01/2019
//

using CityScover.Engine.Workers;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.Greedy
{
   internal class NearestNeighborKnapsack : NearestNeighbor
   {
      #region Constructors
      internal NearestNeighborKnapsack()
         : this(null)
      {
      }

      internal NearestNeighborKnapsack(AlgorithmTracker provider)
         : base(provider)
      {
         Type = AlgorithmType.NearestNeighborKnapsack;
      }
      #endregion

      /// <summary>
      /// This implementation is the knapsack style Best Closest Neighbor variant.
      /// Firstly it gets neighbors of point of interest passed by argument.
      /// Secondly it orders that points by score / weight decreasing value.
      /// The weight value is obtained by getting the edge between param node and the neighbor node.
      /// Finally it returns the first not visited node of that point list.
      /// </summary>
      /// <param name="interestPoint">Point of interest</param>
      /// <returns></returns>
      protected override InterestPointWorker GetBestNeighbor(InterestPointWorker interestPoint)
      {
         var adjPoiIds = CityMapClone.GetAdjacentNodes(interestPoint.Entity.Id);

         var tempNodes = new Collection<(int NodeKey, double Ratio)>();
         adjPoiIds.ToList().ForEach(adjPoiId =>
         {
            var node = CityMapClone[adjPoiId];
            if (node.IsVisited)
            {
               return;
            }

            RouteWorker edge = CityMapClone.GetEdge(interestPoint.Entity.Id, adjPoiId);
            if (edge is null)
            {
               return;
            }

            var value = node.Entity.Score.Value / edge.Weight.Invoke();
            tempNodes.Add((adjPoiId, value));
         });

         var tempNodesSorted = from node in tempNodes
                               orderby node.Ratio descending
                               select node.NodeKey;

         InterestPointWorker candidateNode = default;
         int candidateNodeId = tempNodesSorted.FirstOrDefault();
         if (candidateNodeId != 0)
         {
            candidateNode = CityMapClone[candidateNodeId];
         }

         return candidateNode;
      }
   }
}
