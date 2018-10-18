//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 19/10/2018
//

using System;

namespace CityScover.Entities
{
   public struct IntervalTime
   {
      #region Constructors
      public IntervalTime(DateTime openingTime, DateTime closingTime)
      {
         OpeningTime = openingTime;
         ClosingTime = closingTime;
      } 
      #endregion

      #region Public properties
      public DateTime? OpeningTime { get; }
      public DateTime? ClosingTime { get; }
      #endregion

      #region Public methods
      public IntervalTime DeepCopy() => (IntervalTime)MemberwiseClone();
      #endregion
   }
}
