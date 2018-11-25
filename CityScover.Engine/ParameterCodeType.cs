//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 25/11/2018
//

namespace CityScover.Engine
{
   #region ParameterCodes enumeration
   public enum ParameterCodes
   {
      /// <summary>
      /// Flag that states if an algorithm can execute an improvement logic.
      /// </summary>
      CanDoImprovements,

      /// <summary>
      /// Relaxed constraints for this algorithm.
      /// </summary>
      RelaxedConstraints,
      
      /// <summary>
      /// The Lambda parameter to set the score term (and the distance term) weight.
      /// It is used into the convex combination into objective function.
      /// It has values between 0 and 1.
      /// </summary>
      ObjectiveFunctionScoreWeight,

      /// <summary>
      /// Maximum number of nodes to add to the tour for Greedy algorithms.
      /// </summary>
      GREEDYmaxNodesToAdd,

      /// <summary>
      /// Stopping condition for Local Search algorithm. 
      /// It verifies if there are one or more iterations without 
      /// solution cost improvements during algorithm's execution.
      /// </summary>
      LSmaxRunsWithNoImprovements,

      /// <summary>
      /// Lin Kernighan threshold to trigger improvement logic.
      /// </summary>
      LSimprovementThreshold,

      /// <summary>
      /// Hybrid Distance Insertion algorithm's threshold for Tmax constraint.
      /// </summary>
      HDIthresholdToTmax,

      /// <summary>
      /// Hybrid Distance Insertion algorithm's threshold for time walk.
      /// </summary>
      HDItimeWalkThreshold,

      /// <summary>
      /// Stopping condition for MetaHeuristic algorithm. 
      /// It verifies if there is a deadlock in the algorithm's execution.
      /// </summary>
      TABUmaxDeadlockIterations,

      /// <summary>
      /// Divisor used to calculate the real tabu tenure parameter in Tabu Search algorithm.
      /// The formula to calculate the tabu tenure is (N / TabuTenureFactor), where N 
      /// is the problem size.
      /// Best range for this parameter is in range [2 - 4] for 2-Opt moves 
      /// and [8 - 16] for 3-Opt moves.
      /// </summary>
      TABUtenureFactor
   }
   #endregion
}