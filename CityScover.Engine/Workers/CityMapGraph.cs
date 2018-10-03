//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 03/10/2018
//

using CityScover.ADT.Graphs;
using System;

namespace CityScover.Engine.Workers
{
   internal sealed class CityMapGraph : Graph<int, InterestPointWorker, RouteWorker>
   {
      #region Internal methods
      /// <summary>
      /// A deep copy of the current CityMapGraph object.
      /// </summary>
      /// <returns>A CityMapGraph object</returns>
      internal CityMapGraph DeepCopy()
      {
         CityMapGraph copy = new CityMapGraph();
         foreach (var node in Nodes)
         {
            InterestPointWorker copyInterestPointWorker = node.DeepCopy();
            copy.AddNode(node.Entity.Id, copyInterestPointWorker);
         }

         foreach (var node in Nodes)
         {
            var edges = GetEdges(node.Entity.Id);
            foreach (var edge in edges)
            {
               RouteWorker copyRouteWorker = edge.DeepCopy();
               copy.AddEdge(edge.Entity.PointFrom.Id, edge.Entity.PointTo.Id, copyRouteWorker);
            }
         }
         return copy;
      }
      
      internal void AddRouteFromGraph(CityMapGraph source, int fromPOIKey, int toPOIKey)
      {
         if (source == null)
         {
            throw new ArgumentNullException();
         }

         RouteWorker edge = source.GetEdge(fromPOIKey, toPOIKey);
         if (edge == null)
         {
            throw new InvalidOperationException();
         }

         AddEdge(fromPOIKey, toPOIKey, edge);
      }
      #endregion
   }
}
