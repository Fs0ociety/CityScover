//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 30/09/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.Greedy
{
   internal class NearestNeighborKnapsackAlgorithm : NearestNeighborAlgorithm
   {
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
         var adjPOIIds = _cityMapClone.GetAdjacentNodes(interestPoint.Entity.Id);

         var tempNodes = new Collection<(int, double)>();
         adjPOIIds.ToList().ForEach(adjPOIId => AddWeightedNode(adjPOIId));

         // First local function: AddWeightedNode
         void AddWeightedNode(int adjPOIId)
         {
            RouteWorker edge = _cityMapClone.GetEdge(interestPoint.Entity.Id, adjPOIId);
            if (edge == null)
            {
               return;
            }

            var node = _cityMapClone[adjPOIId];
            var value = node.Entity.Score.Value / edge.Weight.Invoke();
            tempNodes.Add((adjPOIId, value));
         }

         var tempNodesSorted = from node in tempNodes
                               orderby node.Item2 descending
                               select node.Item1;
         
         return _cityMapClone[tempNodesSorted.FirstOrDefault()];
      }
   }
}
