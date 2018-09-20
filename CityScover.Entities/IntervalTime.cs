//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 20/09/2018
//

using System;

namespace CityScover.Entities
{
   public struct IntervalTime
   {
      #region Constructors
      public IntervalTime(TimeSpan openingTime, TimeSpan closingTime)
      {
         OpeningTime = openingTime;
         ClosingTime = closingTime;
      } 
      #endregion

      #region Public properties
      public TimeSpan? OpeningTime { get; }
      public TimeSpan? ClosingTime { get; }
      #endregion

      #region Public methods
      public IntervalTime DeepCopy() => (IntervalTime)MemberwiseClone();
      #endregion
   }
}
