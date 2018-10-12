//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 12/10/2018
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
      internal TourCategoryType TourCategory { get; set; }
      internal int StartingPointId { get; set; }
      internal int PointsCount { get; set; }
      internal double WalkingSpeed { get; set; }
      internal DateTime ArrivalTime { get; set; }
      internal TimeSpan TourDuration { get; set; }
      internal ProblemFamily CurrentProblem { get; set; }
      public bool AlgorithmMonitoring { get; set; }
      internal ICollection<Stage> Stages { get; set; }      
      #endregion
   }
}