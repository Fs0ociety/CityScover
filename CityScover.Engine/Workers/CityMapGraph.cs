//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/08/2018
//

using CityScover.ADT.Graphs;
using CityScover.Entities;
using System;

namespace CityScover.Engine.Workers
{
   [Serializable]
   internal sealed class CityMapGraph : Graph<int, InterestPointWorker, RouteWorker>
   {
      /// <summary>
      /// A shallow copy of the current CityMapGraph object.
      /// </summary>
      /// <returns></returns>
      internal CityMapGraph ShallowCopy() => (CityMapGraph) MemberwiseClone();

      /// <summary>
      /// A deep copy of the current CityMapGraph object.
      /// </summary>
      /// <returns></returns>
      internal CityMapGraph DeepCopy()
      {
         CityMapGraph copy = ShallowCopy();
         foreach (var node in Nodes)
         {
            InterestPointWorker copyInterestPointWorker = node.DeepCopy();
            copy.AddNode(node.Entity.Id, copyInterestPointWorker);

            foreach (var edge in GetEdges(node.Entity.Id))
            {
               RouteWorker copyRouteWorker = edge.DeepCopy();
               copy.AddUndirectedEdge(edge.Entity.PointFrom.Id, edge.Entity.PointTo.Id, copyRouteWorker);
            }
         }         
         return copy;
      }
   }
}
