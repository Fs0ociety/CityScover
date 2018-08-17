//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represents a mapping of a configuration file XML.
   /// </summary>
   public sealed class Configuration
   {
      #region Constructors
      public Configuration()
      {
         Stages = new Collection<StageType>();
      }
      #endregion

      #region Public properties
      public ICollection<StageType> Stages { get; }
      public int? TourCategoryId { get; set; }
      public TimeSpan? Arrivaltime { get; set; }
      public TimeSpan? TourDuration { get; set; }
      #endregion
   }
}