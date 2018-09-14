// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 14/09/2018
//

using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine.Configs
{
   public static class Configurations
   {
      private static ICollection<Configuration> _configurations;

      static Configurations()
      {
         _configurations = new Collection<Configuration>();
         CreateConfigurations();
         Configs = _configurations;
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
                     StageNo = StageType.StageOne,
                     CurrentAlgorithm = AlgorithmType.NearestNeighbor
                  },
                  new Stage()
                  {
                     StageNo = StageType.StageTwo,
                     CurrentAlgorithm = AlgorithmType.TwoOpt,
                     InnerAlgorithms = new KeyValuePair<AlgorithmType, byte>[]
                     {
                        new KeyValuePair<AlgorithmType, byte>(AlgorithmType.LinKernighan, 1)
                     }
                  },
                  new Stage()
                  {
                     StageNo = StageType.StageThree,
                     CurrentAlgorithm = AlgorithmType.TabuSearch,
                     InnerAlgorithms = new KeyValuePair<AlgorithmType, byte>[]
                     {
                        new KeyValuePair<AlgorithmType, byte>(AlgorithmType.TwoOpt, 1),
                        new KeyValuePair<AlgorithmType, byte>(AlgorithmType.LinKernighan, 1),
                     }
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
                     StageNo = StageType.StageOne,
                     CurrentAlgorithm = AlgorithmType.NearestNeighbor
                  },
                  new Stage()
                  {
                     StageNo = StageType.StageTwo,
                     CurrentAlgorithm = AlgorithmType.TwoOpt                     
                  },
                  new Stage()
                  {
                     StageNo = StageType.StageThree,
                     CurrentAlgorithm = AlgorithmType.TabuSearch,
                     InnerAlgorithms = new KeyValuePair<AlgorithmType, byte>[]
                     {
                        new KeyValuePair<AlgorithmType, byte>(AlgorithmType.TwoOpt, 1)
                     }
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
                     StageNo = StageType.StageOne,
                     CurrentAlgorithm = AlgorithmType.CheapestInsertion,
                     InnerAlgorithms = new KeyValuePair<AlgorithmType, byte>[]
                     {
                        new KeyValuePair<AlgorithmType, byte>(AlgorithmType.LinKernighan, 3)
                     }
                  },
                  new Stage()
                  {
                     StageNo = StageType.StageTwo,
                     CurrentAlgorithm = AlgorithmType.TwoOpt
                  },
                  new Stage()
                  {
                     StageNo = StageType.StageThree,
                     CurrentAlgorithm = AlgorithmType.TabuSearch,
                     InnerAlgorithms = new KeyValuePair<AlgorithmType, byte>[]
                     {
                        new KeyValuePair<AlgorithmType, byte>(AlgorithmType.TwoOpt, 1),
                        new KeyValuePair<AlgorithmType, byte>(AlgorithmType.LinKernighan, 1)
                     }
                  },
               },

               RelaxedConstraintsId = new Collection<byte>() { 1, 2, 3 },
               TourCategory = TourCategoryType.HistoricalAndCultural,
               Arrivaltime = new TimeSpan(9, 0, 0),
               TourDuration = new TimeSpan(6, 0, 0)
            });
      }
      #endregion

      public static ICollection<Configuration> Configs { get; set; }
   }
}
