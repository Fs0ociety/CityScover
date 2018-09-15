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

namespace CityScover.Engine.Configs
{
   public static class RunningConfigs
   {
      private static ICollection<Stage[]> _stages;
      private static ICollection<Configuration> _configurations;

      static RunningConfigs()
      {
         _stages = new Collection<Stage[]>();
         _configurations = new Collection<Configuration>();

         CreateStages();
         CreateConfigurations();
      }

      private static void CreateStages()
      {
         _stages.Add(new Stage[]
         {
            new Stage()
            {
               Description = StageType.StageOne,
               CurrentAlgorithm = AlgorithmType.NearestNeighbor,
               RunningCount = 1,
               Stages = null
            },

            new Stage()
            {
               Description = StageType.StageTwo,
               CurrentAlgorithm = AlgorithmType.TwoOpt,
               RunningCount = 1,
               Stages = null
            },

            new Stage()
            {
               Description = StageType.StageThree,
               CurrentAlgorithm = AlgorithmType.TabuSearch,
               RunningCount = 1,
               Stages = new Collection<Stage>()
               {
                  new Stage()
                  {
                     Description = StageType.StageTwo,
                     CurrentAlgorithm = AlgorithmType.TwoOpt,
                     RunningCount = 1,
                     Stages = new Collection<Stage>()
                     {
                        new Stage()
                        {
                           Description = StageType.Improvement,
                           CurrentAlgorithm = AlgorithmType.LinKernighan,
                           RunningCount = 1,
                           Stages = null
                        }
                     }
                  }
               }
            }
         });

         _stages.Add(new Stage[]
         {
            new Stage()
            {
               Description = StageType.StageOne,
               CurrentAlgorithm = AlgorithmType.CheapestInsertion,
               RunningCount = 1,
               Stages = null
            },

            new Stage()
            {
               Description = StageType.StageTwo,
               CurrentAlgorithm = AlgorithmType.TwoOpt,
               RunningCount = 1,
               Stages = new Collection<Stage>()
               {
                  new Stage()
                  {
                     Description = StageType.Improvement,
                     CurrentAlgorithm = AlgorithmType.LinKernighan,
                     RunningCount = 3,
                     Stages = null
                  }
               }
            },

            new Stage()
            {
               Description = StageType.StageThree,
               CurrentAlgorithm = AlgorithmType.TabuSearch,
               RunningCount = 1,
               Stages = new Collection<Stage>()
               {
                  new Stage()
                  {
                     Description = StageType.StageTwo,
                     CurrentAlgorithm = AlgorithmType.TwoOpt,
                     RunningCount = 2,
                     Stages = new Collection<Stage>()
                     {
                        new Stage()
                        {
                           Description = StageType.Improvement,
                           CurrentAlgorithm = AlgorithmType.LinKernighan,
                           RunningCount = 3,
                           Stages = null
                        }
                     }
                  }
               }
            }
         });
      }

      #region Private static methods
      private static void CreateConfigurations()
      {
         _configurations.Add(
            new Configuration()
            {
               CurrentProblem = ProblemType.TeamOrienteering,
               PointsCount = 15,
               Stages = new Collection<Stage>()
               {
                  new Stage()
                  {
                     Description = StageType.StageOne,
                     CurrentAlgorithm = AlgorithmType.NearestNeighbor
                  },
                  new Stage()
                  {
                     Description = StageType.StageTwo,
                     CurrentAlgorithm = AlgorithmType.TwoOpt
                  },
                  new Stage()
                  {
                     Description = StageType.StageThree,
                     CurrentAlgorithm = AlgorithmType.TabuSearch
                  },
               },

               RelaxedConstraintsId = new Collection<byte>() { 1, 2, 3 },
               TourCategory = TourCategoryType.HistoricalAndCultural,
               Arrivaltime = new TimeSpan(9, 0, 0),
               TourDuration = new TimeSpan(6, 0, 0)
            });

         _configurations.Add(
            new Configuration()
            {
               CurrentProblem = ProblemType.TeamOrienteering,
               PointsCount = 15,
               Stages = new Collection<Stage>()
               {
                  new Stage()
                  {
                     Description = StageType.StageOne,
                     CurrentAlgorithm = AlgorithmType.NearestNeighbor
                  },
                  new Stage()
                  {
                     Description = StageType.StageTwo,
                     CurrentAlgorithm = AlgorithmType.TwoOpt
                  },
                  new Stage()
                  {
                     Description = StageType.StageThree,
                     CurrentAlgorithm = AlgorithmType.TabuSearch
                  },
               },

               RelaxedConstraintsId = new Collection<byte>() { 1, 2, 3 },
               TourCategory = TourCategoryType.HistoricalAndCultural,
               Arrivaltime = new TimeSpan(9, 0, 0),
               TourDuration = new TimeSpan(6, 0, 0)
            });

         _configurations.Add(
            new Configuration()
            {
               CurrentProblem = ProblemType.TeamOrienteering,
               PointsCount = 15,
               Stages = new Collection<Stage>()
               {
                  new Stage()
                  {
                     Description = StageType.StageOne,
                     CurrentAlgorithm = AlgorithmType.CheapestInsertion
                  },
                  new Stage()
                  {
                     Description = StageType.StageTwo,
                     CurrentAlgorithm = AlgorithmType.TwoOpt
                  },
                  new Stage()
                  {
                     Description = StageType.StageThree,
                     CurrentAlgorithm = AlgorithmType.TabuSearch
                  },
               },

               RelaxedConstraintsId = new Collection<byte>() { 1, 2, 3 },
               TourCategory = TourCategoryType.HistoricalAndCultural,
               Arrivaltime = new TimeSpan(9, 0, 0),
               TourDuration = new TimeSpan(6, 0, 0)
            });
      }
      #endregion

      public static ICollection<Configuration> Configs => _configurations;
   }
}
