using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Entities
{
   public class Tour
   {
      #region Public properties
      public IList<InterestPoint> Points { get; set; }
      public int TotalScore { get; set; } 
      #endregion
   }
}
