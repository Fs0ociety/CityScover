using System;
using System.Collections.Generic;

namespace CityScover.Entities
{
   public class InterestPoint
   {
      #region Public properties
      public string Name { get; set; }
      public TourCategory Category { get; set; }
      public ThematicScore Score { get; set; }
      public IEnumerable<IntervalTime> OpeningTimes { get; set; }
      public TimeSpan VisitTime { get; set; }
      #endregion
   }
}
