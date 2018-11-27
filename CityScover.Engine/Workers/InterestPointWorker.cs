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

using CityScover.Entities;
using System;

namespace CityScover.Engine.Workers
{
   internal sealed class InterestPointWorker
   {
      #region Constructors
      internal InterestPointWorker(InterestPoint interestPoint)
      {
         Entity = interestPoint;
         TotalTime = DateTime.Now.Date;
      }
      #endregion

      #region Internal properties
      internal InterestPoint Entity { get; private set; }
      internal bool IsVisited { get; set; }
      internal DateTime ArrivalTime { get; set; }
      internal DateTime TotalTime { get; set; }      
      internal TimeSpan WaitOpeningTime { get; set; }      
      #endregion

      #region Internal methods
      internal InterestPointWorker DeepCopy()
      {
         var copy = (InterestPointWorker)MemberwiseClone();
         copy.Entity = Entity.DeepCopy();
         return copy;
      }
      #endregion
   }
}
