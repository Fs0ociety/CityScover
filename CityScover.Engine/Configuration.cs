//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 16/10/2018
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
      internal Configuration()
      {
         Stages = new Collection<Stage>();
      }
      #endregion

      #region Properties
      public TourCategoryType TourCategory { get; set; }
      public int StartingPointId { get; set; }
      public int PointsCount { get; set; }
      public double WalkingSpeed { get; set; }
      public DateTime ArrivalTime { get; set; }
      public TimeSpan TourDuration { get; set; }
      internal ProblemFamily CurrentProblem { get; set; }
      public bool AlgorithmMonitoring { get; set; }
      public ICollection<Stage> Stages { get; set; }      
      #endregion
   }
}