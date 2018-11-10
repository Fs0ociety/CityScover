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

using CityScover.Commons;
using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine.Configs
{
   public static class RunningConfigs
   {
      #region Private fields
      private readonly static ICollection<Configuration> _configurations;
      #endregion

      #region Static Constructors
      static RunningConfigs()
      {
         _configurations = new Collection<Configuration>();
         CreateConfigurationsTest();
      }
      #endregion

      #region Private methods
      private static void CreateConfigurationsTest()
      {
         #region Test - Configuration 1
         Configuration c1Test = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"cityscover-points-30.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(10, 0, 0),
            AlgorithmMonitoring = true,
            Stages =
            {
               new Stage()
               {
                  Description = StageType.StageOne,
                  Category = AlgorithmFamily.Greedy,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.NearestNeighbor,
                     MaximumNodesToEvaluate = 6,
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.HybridNearestDistance, runningCount: 1)
                        {
                           HndTmaxThreshold = new TimeSpan(1, 0, 0)
                        }
                     }
                  }
               },
               new Stage()
               {
                  Description = StageType.StageTwo,
                  Category = AlgorithmFamily.LocalSearch,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.TwoOpt,
                     LkImprovementThreshold = 2000,
                     MaxIterationsWithoutImprovements = 2,
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.LinKernighan, runningCount: 1),
                        new StageFlow(AlgorithmType.HybridNearestDistance, runningCount: 1)
                        {
                           HndTmaxThreshold = new TimeSpan(1, 0, 0)
                        }
                     }
                  }
               },
               new Stage()
               {
                  Description = StageType.StageThree,
                  Category = AlgorithmFamily.MetaHeuristic,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.TabuSearch,
                     MaximumDeadlockIterations = 2,
                     CanExecuteImprovements = false,
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.TwoOpt, runningCount: 3)
                     }
                  }
               }
            }
         };
         #endregion

         #region Test - Configuration 2
         Configuration c1Testv2 = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"cityscover-points-30.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(10, 0, 0),
            AlgorithmMonitoring = true,
            Stages =
            {
               new Stage()
               {
                  Description = StageType.StageOne,
                  Category = AlgorithmFamily.Greedy,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.NearestNeighbor,
                     AlgorithmParameters =
                     {
                        [ParameterCodes.GreedyMaxNodesToAdd] = 6
                     },
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.HybridNearestDistance, runningCount: 1)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.HNDTmaxThreshold] = new TimeSpan(1, 0, 0)
                           }
                        }
                     }
                  }
               },
               new Stage()
               {
                  Description = StageType.StageTwo,
                  Category = AlgorithmFamily.LocalSearch,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.TwoOpt,
                     AlgorithmParameters =
                     {
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.LKImprovementThreshold] = 2000,
                        [ParameterCodes.MaxIterationsWithNoImprovements] = 2,
                     },
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.LinKernighan, runningCount: 1),
                        new StageFlow(AlgorithmType.HybridNearestDistance, runningCount: 1)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.HNDTmaxThreshold] = new TimeSpan(1, 0, 0)
                           }
                        }
                     }
                  }
               },
               new Stage()
               {
                  Description = StageType.StageThree,
                  Category = AlgorithmFamily.MetaHeuristic,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.TabuSearch,
                     AlgorithmParameters =
                     {
                        [ParameterCodes.CanDoImprovements] = false,
                        [ParameterCodes.MaxDeadlockIterations] = 2,
                        [ParameterCodes.TabuTenureFactor] = 2   // Divisor of N where N is the problem size.
                     },
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.TwoOpt, runningCount: 3)
                     }
                  }
               }
            }
         };
         #endregion

         #region Configuration for Test 1
         Configuration c2Test = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"Test.cityscover-test-nearestnbor-5.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(10, 0, 0),
            RelaxedConstraints =
            {
               Utils.TimeWindowsConstraint
            },
            AlgorithmMonitoring = true,
            Stages =
            {
               new Stage()
               {
                  Description = StageType.StageOne,
                  Category = AlgorithmFamily.Greedy,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.NearestNeighbor,
                     MaximumNodesToEvaluate = 6
                  }
               }
            }
         };
         #endregion

         #region Configuration for Test 2
         Configuration c3Test = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"Test.cityscover-test-nnknapsack-5.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(10, 0, 0),
            RelaxedConstraints =
            {
               Utils.TimeWindowsConstraint
            },
            AlgorithmMonitoring = true,
            Stages =
            {
               new Stage()
               {
                  Description = StageType.StageOne,
                  Category = AlgorithmFamily.Greedy,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.NearestNeighborKnapsack,
                     MaximumNodesToEvaluate = 6
                  }
               }
            }
         };
         #endregion

         _configurations.Add(c1Test);
      }
      #endregion

      #region Public properties
      public static ICollection<Configuration> Configurations => _configurations;
      #endregion
   }
}
