//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 10/09/2018
//

namespace CityScover.Engine
{
   public struct Stage
   {
      #region Constructors
      public Stage(StageType stageType)
      {
         StageNo = stageType;
         CurrentAlgorithm = AlgorithmType.None;
      }
      #endregion

      #region Public properties
      public StageType StageNo { get; set; }
      public AlgorithmType CurrentAlgorithm { get; set; }
      #endregion

      #region Public methods
      public static StageType GetStageById(int id)
      {
         StageType stage;

         switch (id)
         {
            case 1:
               stage = StageType.StageOne;
               break;
            case 2:
               stage = StageType.StageTwo;
               break;
            case 3:
               stage = StageType.StageThree;
               break;
            case 4:
               stage = StageType.StageFourth;
               break;
            default:
               stage = StageType.InvalidStage;
               break;
         }
         return stage;
      }

      public static AlgorithmType GetAlgorithmTypeById(int algorithmId)
      {
         AlgorithmType algorithm;

         switch (algorithmId)
         {
            case 1:
               algorithm = AlgorithmType.NearestNeighbor;
               break;

            case 2:
               algorithm = AlgorithmType.NearestInsertion;
               break;

            case 3:
               algorithm = AlgorithmType.CheapestInsertion;
               break;

            case 4:
               algorithm = AlgorithmType.TwoOpt;
               break;

            case 5:
               algorithm = AlgorithmType.CitySwap;
               break;

            case 6:
               algorithm = AlgorithmType.LinKernighan;
               break;

            case 7:
               algorithm = AlgorithmType.IteratedLocalSearch;
               break;

            case 8:
               algorithm = AlgorithmType.TabuSearch;
               break;

            case 9:
               algorithm = AlgorithmType.VariableNeighborhoodSearch;
               break;

            default:
               algorithm = AlgorithmType.None;
               break;
         }
         return algorithm;
      }
      #endregion
   }
}