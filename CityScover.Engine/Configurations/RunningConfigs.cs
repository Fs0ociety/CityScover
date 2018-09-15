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
      private readonly static ICollection<Configuration> _configurations;

      static RunningConfigs()
      {
         _configurations = new Collection<Configuration>();
         CreateConfigurations();
      }

      private static void CreateConfigurations()
      {
         //Configurazione 1
         var config1 = new Configuration();
         config1.CurrentProblem = ProblemType.TeamOrienteering;
         config1.PointsCount = 15;
         config1.TourCategory = TourCategoryType.HistoricalAndCultural;
         config1.RelaxedConstraintsId = new Collection<byte>() { 1, 3, 4 };
         config1.ArrivalTime = new TimeSpan(9, 0, 0);
         config1.TourDuration = new TimeSpan(6, 0, 0);

         // Configurazione 1 - Stage 1
         var conf1Stg1 = new Stage();
         conf1Stg1.Description = StageType.StageOne;         
         conf1Stg1.Flow.Type = AlgorithmType.NearestNeighbor;

         // Configurazione 1 - Stage 2
         var conf1Stg2 = new Stage();
         conf1Stg2.Description = StageType.StageTwo;
         conf1Stg2.Flow.Type = AlgorithmType.TwoOpt;

         // Configurazione 1 - Stage 3
         var conf1Stg3 = new Stage();
         conf1Stg3.Description = StageType.StageThree;
         conf1Stg3.Flow.Type = AlgorithmType.TabuSearch;

         var conf1Stg3Child1 = new StageFlow(AlgorithmType.TwoOpt, 1);

         var conf1Stg3Child1Improvement = new StageFlow(AlgorithmType.LinKernighan, 1);
         conf1Stg3Child1.ChildrenFlows.Add(conf1Stg3Child1Improvement);

         conf1Stg3.Flow.ChildrenFlows.Add(conf1Stg3Child1);

         config1.Stages.Add(conf1Stg1);
         config1.Stages.Add(conf1Stg2);
         config1.Stages.Add(conf1Stg3);

         _configurations.Add(config1);

         //Configurazione 2
         var config2 = new Configuration();
         config2.CurrentProblem = ProblemType.TeamOrienteering;
         config2.PointsCount = 15;
         config2.TourCategory = TourCategoryType.HistoricalAndCultural;
         config2.RelaxedConstraintsId = new Collection<byte>() { 1, 3, 4 };
         config2.ArrivalTime = new TimeSpan(9, 0, 0);
         config2.TourDuration = new TimeSpan(6, 0, 0);

         // Configurazione 2 - Stage 1
         var conf2Stg1 = new Stage();
         conf2Stg1.Description = StageType.StageOne;
         conf2Stg1.Flow.Type = AlgorithmType.CheapestInsertion;

         // Configurazione 2 - Stage 2
         var conf2Stg2 = new Stage();
         conf2Stg2.Description = StageType.StageTwo;
         conf2Stg2.Flow.Type = AlgorithmType.TwoOpt;

         var conf2Stg2Improvement = new StageFlow(AlgorithmType.LinKernighan, 3);
         conf2Stg2.Flow.ChildrenFlows.Add(conf2Stg2Improvement);

         // Configurazione 2 - Stage 3
         var conf2Stg3 = new Stage();
         conf2Stg3.Description = StageType.StageThree;
         conf2Stg3.Flow.Type = AlgorithmType.TabuSearch;

         var conf2Stg3Child1 = new StageFlow(AlgorithmType.TwoOpt, 2);

         var conf2Stg3Child1Improvement = new StageFlow(AlgorithmType.LinKernighan, 3);
         conf2Stg3Child1.ChildrenFlows.Add(conf2Stg3Child1Improvement);

         conf2Stg3.Flow.ChildrenFlows.Add(conf2Stg3Child1);

         config2.Stages.Add(conf2Stg1);
         config2.Stages.Add(conf2Stg2);
         config2.Stages.Add(conf2Stg3);
                  
         _configurations.Add(config2);
      }

      public static ICollection<Configuration> Configs => _configurations;
   }

   //public static class RunningConfigs
   //{
   //   private static ICollection<Stage[]> _stages;
   //   private static ICollection<Configuration> _configurations;

   //   static RunningConfigs()
   //   {
   //      _stages = new Collection<Stage[]>();
   //      _configurations = new Collection<Configuration>();

   //      CreateStages();
   //      CreateConfigurations();
   //   }

   //   private static void CreateStages()
   //   {
   //      _stages.Add(new Stage[]
   //      {
   //         new Stage()
   //         {
   //            Description = StageType.StageOne,
   //            CurrentAlgorithm = AlgorithmType.NearestNeighbor,
   //            RunningCount = 1,
   //            Stages = null
   //         },

   //         new Stage()
   //         {
   //            Description = StageType.StageTwo,
   //            CurrentAlgorithm = AlgorithmType.TwoOpt,
   //            RunningCount = 1,
   //            Stages = null
   //         },

   //         new Stage()
   //         {
   //            Description = StageType.StageThree,
   //            CurrentAlgorithm = AlgorithmType.TabuSearch,
   //            RunningCount = 1,
   //            Stages = new Collection<Stage>()
   //            {
   //               new Stage()
   //               {
   //                  Description = StageType.StageTwo,
   //                  CurrentAlgorithm = AlgorithmType.TwoOpt,
   //                  RunningCount = 1,
   //                  Stages = new Collection<Stage>()
   //                  {
   //                     new Stage()
   //                     {
   //                        Description = StageType.Improvement,
   //                        CurrentAlgorithm = AlgorithmType.LinKernighan,
   //                        RunningCount = 1,
   //                        Stages = null
   //                     }
   //                  }
   //               }
   //            }
   //         }
   //      });

   //      _stages.Add(new Stage[]
   //      {
   //         new Stage()
   //         {
   //            Description = StageType.StageOne,
   //            CurrentAlgorithm = AlgorithmType.CheapestInsertion,
   //            RunningCount = 1,
   //            Stages = null
   //         },

   //         new Stage()
   //         {
   //            Description = StageType.StageTwo,
   //            CurrentAlgorithm = AlgorithmType.TwoOpt,
   //            RunningCount = 1,
   //            Stages = new Collection<Stage>()
   //            {
   //               new Stage()
   //               {
   //                  Description = StageType.Improvement,
   //                  CurrentAlgorithm = AlgorithmType.LinKernighan,
   //                  RunningCount = 3,
   //                  Stages = null
   //               }
   //            }
   //         },

   //         new Stage()
   //         {
   //            Description = StageType.StageThree,
   //            CurrentAlgorithm = AlgorithmType.TabuSearch,
   //            RunningCount = 1,
   //            Stages = new Collection<Stage>()
   //            {
   //               new Stage()
   //               {
   //                  Description = StageType.StageTwo,
   //                  CurrentAlgorithm = AlgorithmType.TwoOpt,
   //                  RunningCount = 2,
   //                  Stages = new Collection<Stage>()
   //                  {
   //                     new Stage()
   //                     {
   //                        Description = StageType.Improvement,
   //                        CurrentAlgorithm = AlgorithmType.LinKernighan,
   //                        RunningCount = 3,
   //                        Stages = null
   //                     }
   //                  }
   //               }
   //            }
   //         }
   //      });
   //   }

   //   #region Private static methods
   //   private static void CreateConfigurations()
   //   {
   //      _configurations.Add(
   //         new Configuration()
   //         {
   //            CurrentProblem = ProblemType.TeamOrienteering,
   //            PointsCount = 15,
   //            Stages = new Collection<Stage>()
   //            {
   //               new Stage()
   //               {
   //                  Description = StageType.StageOne,
   //                  CurrentAlgorithm = AlgorithmType.NearestNeighbor
   //               },
   //               new Stage()
   //               {
   //                  Description = StageType.StageTwo,
   //                  CurrentAlgorithm = AlgorithmType.TwoOpt
   //               },
   //               new Stage()
   //               {
   //                  Description = StageType.StageThree,
   //                  CurrentAlgorithm = AlgorithmType.TabuSearch
   //               },
   //            },

   //            RelaxedConstraintsId = new Collection<byte>() { 1, 2, 3 },
   //            TourCategory = TourCategoryType.HistoricalAndCultural,
   //            Arrivaltime = new TimeSpan(9, 0, 0),
   //            TourDuration = new TimeSpan(6, 0, 0)
   //         });

   //      _configurations.Add(
   //         new Configuration()
   //         {
   //            CurrentProblem = ProblemType.TeamOrienteering,
   //            PointsCount = 15,
   //            Stages = new Collection<Stage>()
   //            {
   //               new Stage()
   //               {
   //                  Description = StageType.StageOne,
   //                  CurrentAlgorithm = AlgorithmType.NearestNeighbor
   //               },
   //               new Stage()
   //               {
   //                  Description = StageType.StageTwo,
   //                  CurrentAlgorithm = AlgorithmType.TwoOpt
   //               },
   //               new Stage()
   //               {
   //                  Description = StageType.StageThree,
   //                  CurrentAlgorithm = AlgorithmType.TabuSearch
   //               },
   //            },

   //            RelaxedConstraintsId = new Collection<byte>() { 1, 2, 3 },
   //            TourCategory = TourCategoryType.HistoricalAndCultural,
   //            Arrivaltime = new TimeSpan(9, 0, 0),
   //            TourDuration = new TimeSpan(6, 0, 0)
   //         });

   //      _configurations.Add(
   //         new Configuration()
   //         {
   //            CurrentProblem = ProblemType.TeamOrienteering,
   //            PointsCount = 15,
   //            Stages = new Collection<Stage>()
   //            {
   //               new Stage()
   //               {
   //                  Description = StageType.StageOne,
   //                  CurrentAlgorithm = AlgorithmType.CheapestInsertion
   //               },
   //               new Stage()
   //               {
   //                  Description = StageType.StageTwo,
   //                  CurrentAlgorithm = AlgorithmType.TwoOpt
   //               },
   //               new Stage()
   //               {
   //                  Description = StageType.StageThree,
   //                  CurrentAlgorithm = AlgorithmType.TabuSearch
   //               },
   //            },

   //            RelaxedConstraintsId = new Collection<byte>() { 1, 2, 3 },
   //            TourCategory = TourCategoryType.HistoricalAndCultural,
   //            Arrivaltime = new TimeSpan(9, 0, 0),
   //            TourDuration = new TimeSpan(6, 0, 0)
   //         });
   //   }
   //   #endregion

   //   public static ICollection<Configuration> Configs => _configurations;
   //}
}
