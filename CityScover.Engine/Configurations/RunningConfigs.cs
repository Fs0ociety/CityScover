//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 05/11/2018
//

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
         CreateConfigurations();
      }
      #endregion

      #region Private methods
      private static void CreateConfigurations()
      {
         #region Configuration 1
         Configuration c1 = new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = TourCategoryType.HistoricalAndCultural,
            PointsFilename = @"Tests.cityscover-points-30.xml",
            StartingPointId = 1,
            WalkingSpeed = 3.0 / 3.6,  // in m/s.
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(10, 0, 0),
            AlgorithmMonitoring = true,
            Stages =
            {
               new Stage()
               {
                  Description = StageType.StageImprovement,
                  Category = AlgorithmFamily.Greedy,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.NearestNeighbor,
                     MaximumNodesToEvaluate = 5
                  }
               },
               //new Stage()
               //{
               //   Description = StageType.StageTwo,
               //   Category = AlgorithmFamily.LocalSearch,
               //   Flow =
               //   {
               //      CurrentAlgorithm = AlgorithmType.TwoOpt,
               //      LkImprovementThreshold = 2000,
               //      MaxIterationsWithoutImprovements = 2,
               //      ChildrenFlows =
               //      {
               //         new StageFlow(AlgorithmType.LinKernighan, runningCount: 1)
               //      }
               //   }
               //},
               new Stage()
               {
                  Description = StageType.StageTwo,
                  Category = AlgorithmFamily.Improvement,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.HybridNearestDistance,
                     HndTmaxThreshold = new TimeSpan(1, 0, 0)
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

         #region Configuration 2
         //_configurations.Add(new Configuration()
         //{
         //   CurrentProblem = ProblemType.TeamOrienteering,
         //   PointsCount = 15,
         //   StartPOIId = 1,
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
         //   ArrivalTime = new TimeSpan(9, 0, 0),
         //   TourDuration = new TimeSpan(6, 0, 0),
         //   AlgorithmMonitoring = true,
         //   Stages =
         //   {
         //      new Stage()
         //      {
         //         Description = StageType.StageOne,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.NearestInsertion
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageTwo,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TwoOpt
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageThree,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TabuSearch,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.TwoOpt, 1)
         //            }
         //         }
         //      }
         //   }
         //});
         #endregion

         #region Configuration 3
         //_configurations.Add(new Configuration()
         //{
         //   CurrentProblem = ProblemType.TeamOrienteering,
         //   PointsCount = 15,
         //   StartPOIId = 1,
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
         //   ArrivalTime = new TimeSpan(9, 0, 0),
         //   TourDuration = new TimeSpan(6, 0, 0),
         //   AlgorithmMonitoring = true,
         //   Stages =
         //   {
         //      new Stage()
         //      {
         //         Description = StageType.StageOne,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.CheapestInsertion
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageTwo,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TwoOpt
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageThree,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TabuSearch,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.TwoOpt, 1)
         //            }
         //         }
         //      }
         //   }
         //});
         #endregion

         #region Configuration 4
         //_configurations.Add(new Configuration()
         //{
         //   CurrentProblem = ProblemType.TeamOrienteering,
         //   PointsCount = 15,
         //   StartPOIId = 1,
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
         //   ArrivalTime = new TimeSpan(9, 0, 0),
         //   TourDuration = new TimeSpan(6, 0, 0),
         //   AlgorithmMonitoring = true,
         //   Stages =
         //   {
         //      new Stage()
         //      {
         //         Description = StageType.StageOne,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.NearestNeighbor
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageTwo,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TwoOpt,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.LinKernighan, 1)
         //            }
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageThree,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TabuSearch,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.TwoOpt, 1)
         //            }
         //         }
         //      }
         //   }
         //});
         #endregion

         #region Configuration 5
         //_configurations.Add(new Configuration()
         //{
         //   CurrentProblem = ProblemType.TeamOrienteering,
         //   PointsCount = 15,
         //   StartPOIId = 1,
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
         //   ArrivalTime = new TimeSpan(9, 0, 0),
         //   TourDuration = new TimeSpan(6, 0, 0),
         //   AlgorithmMonitoring = true,
         //   Stages =
         //   {
         //      new Stage()
         //      {
         //         Description = StageType.StageOne,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.NearestNeighbor
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageTwo,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.CitySwap,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.LinKernighan, 1)
         //            }
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageThree,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TabuSearch,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.TwoOpt, 1)
         //            }
         //         }
         //      }
         //   }
         //});
         #endregion

         #region Configuration 6
         //_configurations.Add(new Configuration()
         //{
         //   CurrentProblem = ProblemType.TeamOrienteering,
         //   PointsCount = 15,
         //   StartPOIId = 1,
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
         //   ArrivalTime = new TimeSpan(9, 0, 0),
         //   TourDuration = new TimeSpan(6, 0, 0),
         //   AlgorithmMonitoring = true,
         //   Stages =
         //   {
         //      new Stage()
         //      {
         //         Description = StageType.StageOne,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.NearestInsertion
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageTwo,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TwoOpt,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.LinKernighan, 1)
         //            }
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageThree,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TabuSearch,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.TwoOpt, 1)
         //            }
         //         }
         //      }
         //   }
         //});
         #endregion

         #region Configuration 7
         //_configurations.Add(new Configuration()
         //{
         //   CurrentProblem = ProblemType.TeamOrienteering,
         //   PointsCount = 15,
         //   StartPOIId = 1,
         //   TourCategory = TourCategoryType.HistoricalAndCultural
         //   RelaxedConstraintsId = { 1, 3, 4 },
         //   ArrivalTime = new TimeSpan(9, 0, 0),
         //   TourDuration = new TimeSpan(6, 0, 0),
         //   AlgorithmMonitoring = true,
         //   Stages =
         //   {
         //      new Stage()
         //      {
         //         Description = StageType.StageOne,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.NearestInsertion
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageTwo,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.CitySwap,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.LinKernighan, 1)
         //            }
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageThree,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TabuSearch,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.TwoOpt, 1)
         //            }
         //         }
         //      }
         //   }
         //});
         #endregion

         #region Configuration 8
         //_configurations.Add(new Configuration()
         //{
         //   CurrentProblem = ProblemType.TeamOrienteering,
         //   PointsCount = 15,
         //   StartPOIId = 1,
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
         //   ArrivalTime = new TimeSpan(9, 0, 0),
         //   TourDuration = new TimeSpan(6, 0, 0),
         //   AlgorithmMonitoring = true,
         //   Stages =
         //   {
         //      new Stage()
         //      {
         //         Description = StageType.StageOne,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.CheapestInsertion
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageTwo,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TwoOpt,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.LinKernighan, 1)
         //            }
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageThree,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TabuSearch,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.TwoOpt, 1)
         //            }
         //         }
         //      }
         //   }
         //});
         #endregion

         #region Configuration 9
         //_configurations.Add(new Configuration()
         //{
         //   CurrentProblem = ProblemType.TeamOrienteering,
         //   PointsCount = 15,
         //   StartPOIId = 1,
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
         //   ArrivalTime = new TimeSpan(9, 0, 0),
         //   TourDuration = new TimeSpan(6, 0, 0),
         //   AlgorithmMonitoring = true,
         //   Stages =
         //   {
         //      new Stage()
         //      {
         //         Description = StageType.StageOne,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.CheapestInsertion
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageTwo,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.CitySwap,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.LinKernighan, 1)
         //            }
         //         }
         //      },
         //      new Stage()
         //      {
         //         Description = StageType.StageThree,
         //         Flow =
         //         {
         //            CurrentAlgorithm = AlgorithmType.TabuSearch,
         //            ChildrenFlows =
         //            {
         //               new StageFlow(AlgorithmType.TwoOpt, 1)
         //            }
         //         }
         //      }
         //   }
         //});
         #endregion

         _configurations.Add(c1);
      }
      #endregion

      #region Public properties
      public static ICollection<Configuration> Configurations => _configurations;
      #endregion
   }
}
