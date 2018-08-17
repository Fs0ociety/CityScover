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
      private int? _tourCategoryId;
      private TimeSpan? _arrivalTime;
      private TimeSpan? _tourDuration;

      #region Constructors
      public Configuration()
      {
         Stages = new Collection<StageType>();
      }
      #endregion

      #region Public properties
      public ICollection<StageType> Stages { get; }

      public int? TourCategoryId
      {
         get => _tourCategoryId;
         set
         {
            if (_tourCategoryId != value)
            {
               _tourCategoryId = value;
            }
         }
      }

      public TimeSpan? Arrivaltime
      {
         get => _arrivalTime;
         set
         {
            if (_arrivalTime != value)
            {
               _arrivalTime = value;
            }
         }
      }

      public TimeSpan? TourDuration
      {
         get => _tourDuration;
         set
         {
            if (_tourDuration != value)
            {
               _tourDuration = value;
            }
         }
      }
      #endregion

      #region Public methods
      public void AddStage(StageType stage)
      {
         Stages.Add(stage);
      }
      #endregion
   }
}