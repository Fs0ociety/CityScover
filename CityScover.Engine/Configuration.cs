﻿//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/08/2018
//

using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// This struct represents a mapping of a configuration file XML.
   /// </summary>
   //public struct Configuration
   //{
   //   #region Public properties
   //   public ProblemType CurrentProblem { get; set; }
   //   public ushort PointsCount { get; set; }
   //   public ICollection<Stage> Stages { get; set; }
   //   public ICollection<byte> RelaxedConstraintsId { get; set; }
   //   public TourCategoryType TourCategory { get; set; }
   //   public TimeSpan? ArrivalTime { get; set; }
   //   public TimeSpan? TourDuration { get; set; }
   //   #endregion
   //}

   public class Configuration
   {
      #region Constructors
      public Configuration()
      {
         Stages = new Collection<Stage>();
      }
      #endregion

      #region Public properties
      public ProblemType CurrentProblem { get; set; }
      public ushort PointsCount { get; set; }
      public ICollection<Stage> Stages { get; set; }
      public ICollection<byte> RelaxedConstraintsId { get; set; }
      public TourCategoryType TourCategory { get; set; }
      public TimeSpan? ArrivalTime { get; set; }
      public TimeSpan? TourDuration { get; set; }
      #endregion
   }
}