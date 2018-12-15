//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 10/12/2018
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
                        [ParameterCodes.LSimprovementThreshold] = 2000,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.LSmaxRunsWithNoImprovements] = 2
                     },
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.HybridCustomUpdate)
                        {
                           AlgorithmParameters =
                           {
                              [ParameterCodes.HDIthresholdToTmax] = new TimeSpan(1, 0, 0),
                              [ParameterCodes.HDItimeWalkThreshold] = new TimeSpan(0, 20, 0),
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
                              [ParameterCodes.HDIthresholdToTmax] = new TimeSpan(1, 0, 0),
                              [ParameterCodes.HDItimeWalkThreshold] = new TimeSpan(0, 20, 0),
                              [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8
                           }
                        }
                     },
                     AlgorithmParameters =
                     {
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.GREEDYmaxNodesToAdd] = 6,
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
                              [ParameterCodes.HDIthresholdToTmax] = new TimeSpan(1, 0, 0),
                              [ParameterCodes.HDItimeWalkThreshold] = new TimeSpan(0, 20, 0),
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
                        [ParameterCodes.LSimprovementThreshold] = 200,
                        [ParameterCodes.LSmaxRunsWithNoImprovements] = 2
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
                                    [ParameterCodes.HDIthresholdToTmax] = new TimeSpan(1, 0, 0),
                                    [ParameterCodes.HDItimeWalkThreshold] = new TimeSpan(0, 20, 0),
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
                              [ParameterCodes.LSimprovementThreshold] = 2000,
                              [ParameterCodes.LSmaxRunsWithNoImprovements] = 2
                           }
                        }
                     },
                     AlgorithmParameters =
                     {
                        [ParameterCodes.MaxIterations] = 10,
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.TABUmaxDeadlockIterations] = 5,
                        [ParameterCodes.TABUtenureFactor] = 4,
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
                        [ParameterCodes.GREEDYmaxNodesToAdd] = 6,
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
                                    [ParameterCodes.HDIthresholdToTmax] = new TimeSpan(1, 0, 0),
                                    [ParameterCodes.HDItimeWalkThreshold] = new TimeSpan(0, 30, 0),
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
                              [ParameterCodes.LSimprovementThreshold] = 2000,
                              [ParameterCodes.LSmaxRunsWithNoImprovements] = 2
                           }
                        }
                     },
                     AlgorithmParameters =
                     {
                        [ParameterCodes.MaxIterations] = 30,
                        [ParameterCodes.TABUmaxDeadlockIterations] = 18,
                        [ParameterCodes.TABUtenureFactor] = 4,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.3
                     }
                  }
               }
            }
         };
         #endregion

         #region Configuration for Test 4 - NN + LS
         Configuration c4Test = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
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
                     AlgorithmParameters =
                     {
                        [ParameterCodes.CanDoImprovements] = true,
                        [ParameterCodes.GREEDYmaxNodesToAdd] = 6,
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
                        [ParameterCodes.LSimprovementThreshold] = 2000,
                        [ParameterCodes.ObjectiveFunctionScoreWeight] = 0.8,
                        [ParameterCodes.LSmaxRunsWithNoImprovements] = 2
                     }
                     //ChildrenFlows =
                     //{
                     //   new StageFlow(AlgorithmType.HybridCustomInsertion)
                     //   {
                     //      AlgorithmParameters =
                     //      {
                     //         [ParameterCodes.CanDoImprovements] = true,
                     //         [ParameterCodes.HDIthresholdToTmax] = new TimeSpan(1, 0, 0),
                     //         [ParameterCodes.HDItimeWalkThreshold] = new TimeSpan(0, 20, 0)
                     //      }
                     //   }
                     //}
                  }
               },
            }
         };
         #endregion

         #region Configuration test 5 - HDI + HDU
         //Configuration c5 = new Configuration()
         //{
         //   CurrentProblem = ProblemFamily.TeamOrienteering,
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
         //   PointsFilename = @"cityscover-points-30.xml",
         //   StartingPointId = 1,
         //   WalkingSpeed = 3.0 / 3.6,  // in m/s.
         //   ArrivalTime = DateTime.Now.Date.AddHours(9),
         //   TourDuration = new TimeSpan(10, 0, 0),
         //   AlgorithmMonitoring = true,
         //   Stages =
         //   {
         //      new Stage()
         //      {
         //         Description = StageType.StageOne,
         //         Category = AlgorithmFamily.Greedy,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.NearestNeighbor,
         //            AlgorithmParameters =
         //            {
         //               [ParameterCodes.CanDoImprovements] = true,
         //               [ParameterCodes.GREEDYmaxNodesToAdd] = 6
         //            },
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.HybridCustomInsertion)
         //               {
         //                  AlgorithmParameters =
         //                  {
         //                     [ParameterCodes.HDIthresholdToTmax] = new TimeSpan(1, 0, 0),
         //                     [ParameterCodes.HDItimeWalkThreshold] = new TimeSpan(0, 20, 0)
         //                  }
         //               }
         //            }
         //         }
         //      }
         //   }
         //};
         #endregion

         Configurations.Add(c1Test);
      }
      #endregion

      #region Public properties
      public static ICollection<Configuration> Configurations { get; }
      #endregion
   }
}
