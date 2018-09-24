//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/09/2018
//

using CityScover.Entities;
using System;

namespace CityScover.Engine.Workers
{
   internal sealed class InterestPointWorker
   {
      private InterestPoint _entity;
      private bool _isVisited;

      internal InterestPointWorker(InterestPoint interestPoint)
      {
         _entity = interestPoint;
      }

      #region Internal properties
      internal InterestPoint Entity
      {
         get => _entity;
         set => _entity = value;         
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
      #endregion

      #region Internal methods
      internal InterestPointWorker DeepCopy()
      {
         InterestPointWorker copy = (InterestPointWorker)MemberwiseClone();
         copy.Entity = Entity.DeepCopy();
         return copy;
      }
      #endregion
   }
}
