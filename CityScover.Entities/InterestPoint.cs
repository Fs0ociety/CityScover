using System;

namespace CityScover.Entities
{
   public class InterestPoint
   {
      #region Public properties
      public string Name { get; set; }
      public TourCategory Category { get; set; }
      public ThematicScore Score { get; set; }
      public IntervalTime OpeningTimeAM { get; set; }
      public IntervalTime OpeningTimePM { get; set; }
      public TimeSpan VisitTime { get; set; }
      #endregion
   }
}
