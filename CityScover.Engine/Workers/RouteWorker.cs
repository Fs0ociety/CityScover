//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/09/2018
//

using System;
using CityScover.ADT.Graphs;
using CityScover.Entities;

namespace CityScover.Engine.Workers
{
   internal sealed class RouteWorker : IGraphEdgeWeight
   {
      private bool _isVisited;

      #region Constructors
      internal RouteWorker(Route route)
      {
         Entity = route;
      } 
      #endregion

      #region Internal properties
      internal Route Entity { get; private set; }      

      internal bool IsVisited
      {
         get => _isVisited;
         set
         {
            if (_isVisited != value)
            {
               _isVisited = value;
            }
         }
      }
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
