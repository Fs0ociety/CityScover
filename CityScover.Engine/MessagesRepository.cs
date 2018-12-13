//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 13/12/2018
//

using System.Collections.Generic;

namespace CityScover.Engine
{
   internal static class MessagesRepository
   {
      private static readonly IDictionary<MessageCode, string> AlgorithmMessages;

      #region Static constructors
      static MessagesRepository()
      {
         AlgorithmMessages = new Dictionary<MessageCode, string>()
         {
            [MessageCode.None] = string.Empty,
            [MessageCode.GreedyStart] = "Greedy {0} algorithm starts...",
            [MessageCode.GreedyNodeAdded] = "Point of interest \"{0}\" added to solution {1}.",
            [MessageCode.GreedyNodeRemoved] = "Point of interest \"{0}\" removed from tour.",
            [MessageCode.GreedyStop] = "Greedy algorithm finished successfully.",

            [MessageCode.LocalSearchStartSolution] = "Local Search starting with solution {0} with cost: {1}",
            [MessageCode.LocalSearchResumeSolution] = "Resume Local Search with solution {0} with total cost {1}.",
            [MessageCode.LocalSearchNewNeighborhood] = "Generating new neighborhood {0}.",
            [MessageCode.LocalSearchNewNeighborhoodMove] = "Adding new solution {0} to neighborhood {1}.",
            [MessageCode.LocalSearchNewNeighborhoodMoveDetails] = "New solution {0} details: swapping between edge {1} and edge {2}.\nSwap produces new edges {3} and {4}.",
            [MessageCode.LocalSearchNeighborhoodBest] = "The best move for this neighborhood is \"{0}\" with cost --> {1}.",
            [MessageCode.LocalSearchBestFound] = "Best solution found with total cost: ({0}). Previous solution cost --> ({1}).",
            [MessageCode.LocalSearchInvariateSolution] = "For this neighborhood a better solution of current best (id: {0} , cost: {1}), hasn't found...",
            [MessageCode.LocalSearchImprovementsPerformed] = "Local Search has performed a total of {0} improvements.",
            [MessageCode.LocalSearchStop] = "Local Search didn't find a better solution and it stops with current solution {0} with cost --> {1}",

            [MessageCode.HybridDistanceInsertionStart] = "Hybrid Distance Insertion improvement algorithm starts...",
            [MessageCode.HybridDistanceInsertionNewNodeAdded] = "Point of interest \"{0}\" added successfully to the tour.",
            [MessageCode.HybridDistanceInsertionNewNodeRemoved] = "Point of interest \"{0}\" removed from the Tour.",
            [MessageCode.HybridDistanceInsertionStopWithSolution] = "Hybrid Distance Insertion STOP condition occurred with solution {0} with total cost --> {1}",
            [MessageCode.HybridDistanceInsertionStopWithoutSolution] = "Hybrid Distance Insertion STOP condition occurred. Exit.",

            [MessageCode.HybridDistanceUpdateStart] = "Hybrid Distance Update improvement algorithm starts...",
            [MessageCode.HybridDistanceUpdateTourUpdated] = "Point of interest \"{0}\" replaced with point of interest \"{1}\".",
            [MessageCode.HybridDistanceUpdateTourRestored] = "Point of interest \"{0}\" removed. Tour restored with \"{1}\".",
            [MessageCode.HybridDistanceUpdateStopWithSolution] = "Hybrid Distance Update STOP condition occurred with solution {0} with total cost --> {1}",
            [MessageCode.HybridDistanceUpdateStopWithoutSolution] = "Hybrid Distance Update STOP condition occurred. Tour not updated! Exit.",

            [MessageCode.LinKernighanStartSolution] = "Lin Kernighan starting with current Local Search Best Solution {0} with cost {1}.",
            [MessageCode.LinKernighanHStepIncreased] = "Lin Kernighan Step {0} of {1}.",
            [MessageCode.LinKernighanStart] = "Max iterations with no improvements reached. Starting Lin Kernighan...",
            [MessageCode.LinKernighanBlockedMove] = "Move {0} already selected! Continue searching...",
            [MessageCode.LinKernighanNoSNodeSelected] = "No one move can be selected! Terminating...",
            [MessageCode.LinKernighanBestFound] = "Best solution found with total cost: ({0}).",
            [MessageCode.LinKernighanInvariateSolution] = "A better solution of current best (id: {0} , total cost: {1}), hasn't found. Terminating...",
            [MessageCode.LinKernighanStop] = "Lin Kernighan algorithm finished successfully.",

            [MessageCode.TabuSearchStart] = "Tabu Search starts with solution {0} with cost: {1}",
            [MessageCode.TabuSearchBestFound] = "Tabu Search has found a new best solution found with total cost: ({0}). Previous solution total cost: ({1}).",
            [MessageCode.TabuSearchMoveLocked] = "Move -> First edge: {0} - Second edge: {1} added in Tabu List. Move locked.",
            [MessageCode.TabuSearchMovesLocked] = "Tabu List status -> First edge: {0} - Second edge: {1} Expiration: {2}, Tenure: {3}",
            [MessageCode.TabuSearchMoveUnlocked] = "Reverse move unlocked -> First edge: {0} - Second edge: {1} removed from Tabu List. Expiration {2}",
            [MessageCode.TabuSearchImprovementsPerformed] = "Tabu Search has performed a total of {0} improvements.",
            [MessageCode.TabuSearchStop] = "Tabu Search STOP condition occurred with solution {0} with total cost --> {1}",

            [MessageCode.ALCompletionSummary] = "EXECUTION SUMMARY",
            [MessageCode.EXREPSolutionReceived] = "Solution {0} has been marked with cost: {1}",
            [MessageCode.EXREPSolutionReceivedWithPenalty] = "Solution {0} has been marked with cost: {1} and penalty: {2}",
            [MessageCode.EXREPAlgorithmPerformance] = "The algorithm {0} performed in {1}.",
            [MessageCode.EXREPExceptionOccurred] = "Exception occurred: {0}.\n Stack trace: {1}\n",
            [MessageCode.EXREPTimeFormat] = "{0} Hours, {1} Minutes, {2} Seconds.",
            [MessageCode.ViolatedConstraints] = "Violated Constraints detail",
            [MessageCode.ViolatedConstraint] = "{0} --> VIOLATED! ",
            [MessageCode.SolverStageStart] = "Starting Stage \"{0}\"...",
            [MessageCode.SolverStageEnd] = "Stage \"{0}\" completed.\n",
            [MessageCode.SolverGraphCreationError] = "Empty graph. Error during creation of the graph in method \"{0}\".",
            [MessageCode.TOSolutionCollectionId] = "Solution {0}\n",
            [MessageCode.CMGraphNodeToString] = "{0} at {1}",
            [MessageCode.TOSolutionFinalTour] = "The final best tour is solution \"{0}\":\n{1}",
            [MessageCode.TOSolutionTotalTimeAndValidity] = "Cost: {0}. Tour time: {1} hours and {2} minutes. Admissible: {3}."
         };
      }
      #endregion

      #region Internal methods
      internal static string GetMessage(MessageCode code, params object[] messageList)
      {
         return !AlgorithmMessages.ContainsKey(code) 
            ? string.Empty 
            : string.Format(AlgorithmMessages[code], messageList);
      }
      #endregion
   }

   #region MessageCode enumeration
   internal enum MessageCode
   {
      None,
      GreedyStart,
      GreedyNodeAdded,
      GreedyNodeRemoved,
      GreedyStop,
      HybridDistanceInsertionStart,
      HybridDistanceInsertionNewNodeAdded,
      HybridDistanceInsertionNewNodeRemoved,
      HybridDistanceInsertionStopWithSolution,
      HybridDistanceInsertionStopWithoutSolution,
      HybridDistanceUpdateStart,
      HybridDistanceUpdateTourUpdated,
      HybridDistanceUpdateTourRestored,
      HybridDistanceUpdateStopWithSolution,
      HybridDistanceUpdateStopWithoutSolution,
      LocalSearchStartSolution,
      LocalSearchResumeSolution,
      LocalSearchNewNeighborhood,
      LocalSearchNewNeighborhoodMove,
      LocalSearchNewNeighborhoodMoveDetails,
      LocalSearchNeighborhoodBest,
      LocalSearchBestFound,
      LocalSearchInvariateSolution,
      LocalSearchImprovementsPerformed,
      LocalSearchStop,
      LinKernighanStartSolution,
      LinKernighanHStepIncreased,
      LinKernighanStart,
      LinKernighanBlockedMove,
      LinKernighanNoSNodeSelected,
      LinKernighanBestFound,
      LinKernighanInvariateSolution,
      LinKernighanStop,
      TabuSearchStart,
      TabuSearchBestFound,
      TabuSearchMoveLocked,
      TabuSearchMovesLocked,
      TabuSearchMoveUnlocked,
      TabuSearchImprovementsPerformed,
      TabuSearchStop,
      ALCompletionSummary,
      EXREPSolutionReceived,
      EXREPSolutionReceivedWithPenalty,
      EXREPAlgorithmPerformance,
      EXREPExceptionOccurred,
      EXREPTimeFormat,
      ViolatedConstraints,
      ViolatedConstraint,
      SolverStageStart,
      SolverStageEnd,
      SolverGraphCreationError,
      TOSolutionCollectionId,
      CMGraphNodeToString,
      TOSolutionFinalTour,
      TOSolutionTotalTimeAndValidity
   }
   #endregion
}
