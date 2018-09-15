﻿//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 15/09/2018
//

using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represents a mapping of a configuration.
   /// </summary>
   public class Configuration
   {
      #region Constructors
      public Configuration()
      {
         Stages = new Collection<Stage>();
         RelaxedConstraintsId = new Collection<byte>();
      }
      #endregion

      #region Public properties
      public ProblemType CurrentProblem { get; set; }
      public ICollection<byte> RelaxedConstraintsId { get; set; }
      public TourCategoryType TourCategory { get; set; }
      public ushort PointsCount { get; set; }
      public TimeSpan? ArrivalTime { get; set; }
      public TimeSpan? TourDuration { get; set; }
      public ICollection<Stage> Stages { get; set; }
      #endregion
   }
}