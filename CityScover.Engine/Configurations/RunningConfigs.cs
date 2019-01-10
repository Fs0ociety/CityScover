//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 19/12/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CityScover.Commons;
using CityScover.Entities;

namespace CityScover.Engine.Configurations
{
   public static class RunningConfigs
   {
      #region Static Constructors
      static RunningConfigs()
      {
         Configurations = new Collection<Configuration>();
         CreateConfigurationsTest();
      }
      #endregion

      #region Private methods
      private static void CreateConfigurationsTest()
      {
         #region Test Screenshot NN
         Configuration testScreenshotNN = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"cityscover-points-30.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(6, 0, 0),
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
                        [ParameterCodes.CanDoImprovements] = false,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.RelaxedConstraints] = new Collection<string>()
                        {
                           Utils.TimeWindowsConstraint
                        }
                     }
                  }
               }               
            }
         };
         #endregion

         #region Test Screenshot NN + Improvement
         Configuration testScreenshotNNImp = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"cityscover-points-30.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(6, 0, 0),
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
                        [ParameterCodes.CanDoImprovements] = false,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.RelaxedConstraints] = new Collection<string>()
                        {
                           Utils.TimeWindowsConstraint
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
                        [ParameterCodes.MaxIterations] = 5,
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.LocalSearchImprovementThreshold] = 2000,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.LocalSearchMaxRunsWithNoImprovements] = 2
                     },
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.HybridCustomUpdate)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.HciTimeThresholdToTmax] = new TimeSpan(1, 0, 0),
                              [ParameterCodes.HcuTimeWalkThreshold] = new TimeSpan(0, 20, 0),
                              [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                           }
                        },
                        new StageFlow(AlgorithmType.LinKernighan)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.MaxIterations] = 20
                           }
                        }
                     }
                  }
               }
            }
         };
         #endregion

         #region Test - Configuration 1
         Configuration c1Test = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.None,
            PointsFilename = @"cityscover-points-90.xml",
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
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.HybridCustomUpdate)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.HciTimeThresholdToTmax] = new TimeSpan(1, 0, 0),
                              [ParameterCodes.HcuTimeWalkThreshold] = new TimeSpan(0, 20, 0),
                              [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                           }
                        }
                     },
                     AlgorithmParameters =
                     {
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.GreedyMaxNodesToAdd] = 6,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.RelaxedConstraints] = new Collection<string>()
                        {
                           Utils.TimeWindowsConstraint
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
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.HybridCustomInsertion)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.HciTimeThresholdToTmax] = new TimeSpan(1, 0, 0),
                              [ParameterCodes.HcuTimeWalkThreshold] = new TimeSpan(0, 20, 0),
                              [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                           }
                        },
                        new StageFlow(AlgorithmType.LinKernighan)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.MaxIterations] = 10
                           }
                        }
                     },
                     AlgorithmParameters =
                     {
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.LocalSearchImprovementThreshold] = 200,
                        [ParameterCodes.LocalSearchMaxRunsWithNoImprovements] = 2
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
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.TwoOpt)
                        {
                           ChildrenFlows =
                           {
                              new StageFlow(AlgorithmType.HybridCustomInsertion)
                              {
                                 AlgorithmParameters =
                                 {
                                    [ParameterCodes.HciTimeThresholdToTmax] = new TimeSpan(1, 0, 0),
                                    [ParameterCodes.HcuTimeWalkThreshold] = new TimeSpan(0, 20, 0),
                                    [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                                 }
                              },
                              new StageFlow(AlgorithmType.LinKernighan)
                              {
                                 AlgorithmParameters =
                                 {
                                    [ParameterCodes.MaxIterations] = 20
                                 }
                              }
                           },
                           AlgorithmParameters =
                           {
                              [ParameterCodes.CanDoImprovements] = true,
                              [ParameterCodes.LocalSearchImprovementThreshold] = 2000,
                              [ParameterCodes.LocalSearchMaxRunsWithNoImprovements] = 2
                           }
                        }
                     },
                     AlgorithmParameters =
                     {
                        [ParameterCodes.MaxIterations] = 10,
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.TabuDeadlockIterations] = 5,
                        [ParameterCodes.TabuTenureFactor] = 4,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                     }
                  }
               }
            }
         };
         #endregion

         #region Test - Configuration 2
         Configuration c2Test = new Configuration()
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
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.GreedyMaxNodesToAdd] = 6,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.RelaxedConstraints] = new Collection<string>()
                        {
                           Utils.TimeWindowsConstraint
                        }
                     }
                  }
               },
               new Stage()
               {
                  Description = StageType.StageTwo,
                  Category = AlgorithmFamily.MetaHeuristic,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.TabuSearch,
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.TwoOpt)
                        {
                           ChildrenFlows =
                           {
                              new StageFlow(AlgorithmType.HybridCustomInsertion)
                              {
                                 AlgorithmParameters =
                                 {
                                    [ParameterCodes.HciTimeThresholdToTmax] = new TimeSpan(1, 0, 0),
                                    [ParameterCodes.HcuTimeWalkThreshold] = new TimeSpan(0, 30, 0),
                                    [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                                 }
                              },
                              new StageFlow(AlgorithmType.LinKernighan)
                              {
                                 AlgorithmParameters =
                                 {
                                    [ParameterCodes.MaxIterations] = 20
                                 }
                              }
                           },
                           AlgorithmParameters =
                           {
                              [ParameterCodes.CanDoImprovements] = true,
                              [ParameterCodes.LocalSearchImprovementThreshold] = 2000,
                              [ParameterCodes.LocalSearchMaxRunsWithNoImprovements] = 2
                           }
                        }
                     },
                     AlgorithmParameters =
                     {
                        [ParameterCodes.MaxIterations] = 30,
                        [ParameterCodes.TabuDeadlockIterations] = 18,
                        [ParameterCodes.TabuTenureFactor] = 4,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.3
                     }
                  }
               }
            }
         };
         #endregion

         #region Test Screenshot NN + LS
         Configuration testScrNNLS = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"cityscover-points-90.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(6, 0, 0),
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
                        [ParameterCodes.CanDoImprovements] = false,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.RelaxedConstraints] = new Collection<string>()
                        {
                           Utils.TimeWindowsConstraint
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
                        [ParameterCodes.MaxIterations] = 5,
                        [ParameterCodes.CanDoImprovements] = false,
                        [ParameterCodes.LocalSearchImprovementThreshold] = 2000,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.LocalSearchMaxRunsWithNoImprovements] = 2
                     }
                  }
               },
            }
         };
         #endregion

         #region Test NN + LS + LK
         Configuration testScrNNLSLK = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"cityscover-points-30.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(6, 0, 0),
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
                        [ParameterCodes.CanDoImprovements] = false,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.RelaxedConstraints] = new Collection<string>()
                        {
                           Utils.TimeWindowsConstraint
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
                        [ParameterCodes.MaxIterations] = 5,
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.LocalSearchImprovementThreshold] = 2000,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.LocalSearchMaxRunsWithNoImprovements] = 1
                     },
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.LinKernighan)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.MaxIterations] = 10,
                              [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                           }
                        }
                     }
                  }
               },
            }
         };
         #endregion

         #region Test - Configuration 5 - NN + HCI/HCU
         Configuration c5Test = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.Sport,
            PointsFilename = @"cityscover-points-30.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(6, 0, 0),
            AlgorithmMonitoring = true,
            Stages =
            {
               new Stage()
               {
                  Description = StageType.StageOne,
                  Category = AlgorithmFamily.Greedy,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.CheapestInsertion,
                     AlgorithmParameters =
                     {
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.RelaxedConstraints] = new Collection<string>()
                        {
                           Utils.TimeWindowsConstraint
                        }
                     },
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.HybridCustomUpdate)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.HciTimeThresholdToTmax] = new TimeSpan(1, 0, 0),
                              [ParameterCodes.HcuTimeWalkThreshold] = new TimeSpan(0, 20, 0),
                              [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                           }
                        }
                     }
                  }
               }
            }
         };
         #endregion

         #region Test NN + LS + LK
         Configuration c1TestLK = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"cityscover-points-60.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(6, 0, 0),
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
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.RelaxedConstraints] = new Collection<string>()
                        {
                           Utils.TimeWindowsConstraint
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
                     ChildrenFlows =
                     {                        
                        new StageFlow(AlgorithmType.LinKernighan)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.MaxIterations] = 10,
                              [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                           }
                        }
                     },
                     AlgorithmParameters =
                     {
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.LocalSearchImprovementThreshold] = 10,
                        [ParameterCodes.LocalSearchMaxRunsWithNoImprovements] = 1
                     }
                  }
               }               
            }
         };
         #endregion

         Configurations.Add(c5Test);
      }
      #endregion

      #region Public properties
      public static ICollection<Configuration> Configurations { get; }
      #endregion
   }
}
