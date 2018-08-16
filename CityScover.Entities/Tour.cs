//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using System.Collections.Generic;

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
