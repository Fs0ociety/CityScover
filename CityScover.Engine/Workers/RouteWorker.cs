//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 27/11/2018
//

using System;
using CityScover.ADT.Graphs;
using CityScover.Entities;

namespace CityScover.Engine.Workers
{
   internal sealed class RouteWorker : IGraphEdgeWeight
   {
      #region Constructors
      internal RouteWorker(Route route)
      {
         Entity = route;
      } 
      #endregion

      #region Internal properties
      internal Route Entity { get; private set; }

      internal bool IsVisited { get; set; }
      #endregion

      #region Internal methods
      internal RouteWorker DeepCopy()
      {
         RouteWorker copy = (RouteWorker)MemberwiseClone();
         copy.Entity = Entity.DeepCopy();
         return copy;
      }
      #endregion

      #region Delegates
      public Func<double> Weight => () => Entity.Distance;
      #endregion
   }
}
