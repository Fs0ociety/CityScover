//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 08/11/2018
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
         //RelaxedConstraints = new Collection<string>();
      }
      #endregion

      #region Properties
      public TourCategoryType TourCategory { get; set; }
      public int StartingPointId { get; set; }
      public string PointsFilename { get; set; }
      public double WalkingSpeed { get; set; }
      public DateTime ArrivalTime { get; set; }
      public TimeSpan TourDuration { get; set; }
      public ProblemFamily CurrentProblem { get; set; }
      public bool AlgorithmMonitoring { get; set; }
      public ICollection<Stage> Stages { get; set; }
      //public ICollection<string> RelaxedConstraints { get; set; }
      #endregion
   }
}