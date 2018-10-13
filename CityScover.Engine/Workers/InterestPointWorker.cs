//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 13/10/2018
//

using CityScover.Entities;

namespace CityScover.Engine.Workers
{
   internal sealed class InterestPointWorker
   {
      #region Private fields
      private InterestPoint _entity;
      private bool _isVisited;
      #endregion

      #region Constructors
      internal InterestPointWorker(InterestPoint interestPoint)
      {
         _entity = interestPoint;
      }
      #endregion

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
