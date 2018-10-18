//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 18/10/2018
//

using CityScover.Entities;
using System;

namespace CityScover.Engine.Workers
{
   internal sealed class InterestPointWorker
   {
      #region Private fields
      private InterestPoint _entity;
      private bool _isVisited;
      private DateTime _arrivalTime;
      private TimeSpan _waitOpeningTime;
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

      internal DateTime ArrivalTime
      {
         get => _arrivalTime;
         set
         {
            if (_arrivalTime != value)
            {
               _arrivalTime = value;
            }
         }
      }

      internal TimeSpan WaitOpeningTime
      {
         get => _waitOpeningTime;
         set
         {
            if (_waitOpeningTime != value)
            {
               _waitOpeningTime = value;
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

      internal DateTime GetTotalTime()
      {
         TimeSpan visitTime = default;
         if (_entity.TimeVisit.HasValue)
         {
            visitTime = _entity.TimeVisit.Value;
         }

         return _arrivalTime
            .Add(_waitOpeningTime)
            .Add(visitTime);
      }
      #endregion
   }
}
