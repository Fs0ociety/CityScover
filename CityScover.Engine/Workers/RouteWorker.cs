//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using System;
using CityScover.ADT.Graphs;
using CityScover.Entities;

namespace CityScover.Engine.Workers
{
   internal sealed class RouteWorker : IGraphEdgeWeight
   {
      private Route _entity;
      private bool _isVisited;

      internal RouteWorker(Route route)
      {
         _entity = route ?? throw new ArgumentNullException(nameof(route));
      }

      #region Internal properties
      internal Route Entity
      {
         get => _entity;
         set
         {
            if (value == null)
            {
               throw new ArgumentNullException(nameof(value));
            }

            if (_entity != value)
            {
               _entity = value;
            }
         }
      }

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

      //TODO: valutare se rimuovere l'implementazione dell'interfaccia IGraphEdgeWeight da Route.
      //TODO: chiedere a Riccardo come gestire visibilita' 
      public Func<double> Weight => _entity.Weight;
      #endregion      
   }
}
