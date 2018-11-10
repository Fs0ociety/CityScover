//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/11/2018
//

namespace CityScover.Engine
{
   #region StageType enumeration
   public enum StageType
   {
      /// <summary>
      /// Invalid Stage type.
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
      /// Stage for Metaheuristic algorithms.
      /// </summary>
      StageThree,

      /// <summary>
      /// Stage for Improvement algorithms.
      /// </summary>
      StageImprovement
   }
   #endregion
}