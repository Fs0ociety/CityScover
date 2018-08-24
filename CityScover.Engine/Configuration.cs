//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/08/2018
//

using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   /// <summary>
   /// This struct represents a mapping of a configuration file XML.
   /// </summary>
   public struct Configuration
   {
      #region Public properties
      public ushort PointsCount { get; set; }
      public ICollection<Stage> Stages { get; set; }
      public ICollection<byte> RelaxedConstraintsId { get; set; }
      public TourCategoryType TourCategory { get; set; }
      public TimeSpan? Arrivaltime { get; set; }
      public TimeSpan? TourDuration { get; set; }
      #endregion
   }
}