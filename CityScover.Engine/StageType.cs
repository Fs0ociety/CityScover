//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 25/08/2018
//

namespace CityScover.Engine
{
   public enum StageType
   {
      /// <summary>
      /// TODO
      /// </summary>
      InvalidStage,

      /// <summary>
      /// Stage for Greedy algorithms.
      /// </summary>
      StageOne,

      /// <summary>
      /// Stage for Local Search algorithms.
      /// </summary>
      StageTwo,

      /// <summary>
      /// Stage for improvements algorithms.
      /// </summary>
      StageThree,

      /// <summary>
      /// Stage for Metaheuristic algorithms.
      /// </summary>
      StageFourth
   }
}