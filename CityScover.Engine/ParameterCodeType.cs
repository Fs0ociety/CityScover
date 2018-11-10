//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 10/11/2018
//

namespace CityScover.Engine
{
   #region ParameterCodes enumeration
   public enum ParameterCodes
   {
      /// <summary>
      /// Maximum number of nodes to add to the tour for Greedy algorithms.
      /// </summary>
      GreedyMaxNodesToAdd,
      
      /// <summary>
      /// Flag that states if an algorithm can execute an improvement logic.
      /// </summary>
      CanDoImprovements,

      /// <summary>
      /// Stopping condition for Local Search algorithm. 
      /// It verifies if there are one or more iterations without 
      /// solution cost improvements during algorithm's execution.
      /// </summary>
      MaxIterationsWithNoImprovements,

      /// <summary>
      /// Stopping condition for MetaHeuristic algorithm. 
      /// It verifies if there is a deadlock in the algorithm's execution.
      /// </summary>
      MaxDeadlockIterations,

      /// <summary>
      /// Lin Kernighan threshold to trigger improvement logic.
      /// </summary>
      LKImprovementThreshold,

      /// <summary>
      /// Hybrid Nearest Distance algorithm's threshold for Tmax constraint.
      /// </summary>
      HNDTmaxThreshold,

      /// <summary>
      /// Divisor used to calculate the real tabu tenure parameter in Tabu Search algorithm.
      /// The formula to calculate the tabu tenure is (N / TabuTenureFactor), where N 
      /// is the problem size.
      /// Best range for this parameter is in range [2 - 4] for 2-Opt moves 
      /// and [8 - 16] for 3-Opt moves.
      /// </summary>
      TabuTenureFactor
   }
   #endregion
}