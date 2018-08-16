//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using System;

namespace CityScover.Entities
{
   public class IntervalTime
   {
      #region Public properties
      public TimeSpan? OpeningTime { get; set; }
      public TimeSpan? ClosingTime { get; set; } 
      #endregion
   }
}
