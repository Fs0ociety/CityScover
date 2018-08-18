//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/08/2018
//

namespace CityScover.Engine
{
   public struct Stage
   {
      public static readonly Stage InvalidStage =
         new Stage(0, "Invalid Stage");

      public static readonly Stage StageOne =
         new Stage(1, "Greedy Stage");

      public static readonly Stage StageTwo =
         new Stage(2, "Local Search Stage");

      public static readonly Stage StageThree =
         new Stage(3, "Local Search Improvement Stage");

      public static readonly Stage StageFour =
         new Stage(4, "Metaheuristic Stage");

      #region Constructors
      private Stage(byte id, string description)
      {
         Id = id;
         Description = description;
         CurrentAlgorithm = AlgorithmType.None;
      }
      #endregion

      #region Public properties
      public byte Id { get; private set; }
      public string Description { get; private set; }
      public AlgorithmType CurrentAlgorithm { get; set; } 
      #endregion

      #region Public methods
      public static Stage GetStageById(int id)
      {
         Stage stage = InvalidStage;

         if (id == StageOne.Id)
         {
            stage = StageOne;
         }
         else if (id == StageTwo.Id)
         {
            stage = StageTwo;
         }
         else if (id == StageThree.Id)
         {
            stage = StageThree;
         }
         else
         {
            stage = StageFour;
         }
         return stage;
      }

      public static AlgorithmType GetAlgorithmTypeById(int algorithmId)
      {
         AlgorithmType algorithm = AlgorithmType.None;

         switch (algorithmId)
         {
            case (int)AlgorithmType.NearestNeighbor:
               algorithm = AlgorithmType.NearestNeighbor;
               break;

            case (int)AlgorithmType.NearestInsertion:
               algorithm = AlgorithmType.NearestInsertion;
               break;

            case (int)AlgorithmType.CheapestInsertion:
               algorithm = AlgorithmType.CheapestInsertion;
               break;

            case (int)AlgorithmType.TwoOpt:
               algorithm = AlgorithmType.TwoOpt;
               break;
            case (int)AlgorithmType.CitySwap:
               algorithm = AlgorithmType.CitySwap;
               break;

            case (int)AlgorithmType.LinKernighan:
               algorithm = AlgorithmType.LinKernighan;
               break;

            case (int)AlgorithmType.IteratedLocalSearch:
               algorithm = AlgorithmType.IteratedLocalSearch;
               break;

            case (int)AlgorithmType.TabuSearch:
               algorithm = AlgorithmType.TabuSearch;
               break;

            case (int)AlgorithmType.VariableNeighborhoodSearch:
               algorithm = AlgorithmType.VariableNeighborhoodSearch;
               break;

            default:
               break;
         }
         return algorithm;
      }
      #endregion
   }
}