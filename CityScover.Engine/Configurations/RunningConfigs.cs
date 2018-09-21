﻿// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 19/09/2018
//

using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine.Configs
{
   public static class RunningConfigs
   {
      private readonly static ICollection<Configuration> _configurations;

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
         _configurations.Add(new Configuration()
         {
            CurrentProblem = ProblemType.TeamOrienteering,
            TourCategory = TourCategoryType.Culinary,
            PointsCount = 15,
            StartPOIId = 1,
            RelaxedConstraintsId = { 1, 3, 4 },
            ArrivalTime = DateTime.Now.Date.AddHours(9),
            TourDuration = new TimeSpan(6, 0, 0),
            AlgorithmMonitoring = true,
            Stages =
            {
               new Stage()
               {
                  Description = StageType.StageOne,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.NearestNeighbor
                  }
               },
               new Stage()
               {
                  Description = StageType.StageTwo,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.TwoOpt
                  }
               },
               new Stage()
               {
                  Description = StageType.StageThree,
                  Flow =
                  {
                     CurrentAlgorithm = AlgorithmType.TabuSearch,
                     ChildrenFlows =
                     {
                        new StageFlow(AlgorithmType.TwoOpt, 1)
                     }
                  }
               }
            }
         });
         #endregion

         #region Configuration 2
         //_configurations.Add(new Configuration()
         //{
         //   CurrentProblem = ProblemType.TeamOrienteering,
         //   PointsCount = 15,
         //   StartPOIId = 1,
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
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
         //   TourCategory = TourCategoryType.HistoricalAndCultural,
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
      }
      #endregion

      #region Public properties
      public static ICollection<Configuration> Configurations => _configurations;
      #endregion
   }
}
