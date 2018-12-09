//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/12/2018
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
            [MessageCode.GreedyNodeAdded] = "Point of interest \"{0}\" added to solution {1}.",
            [MessageCode.GreedyNodeRemoved] = "Point of interest \"{0}\" removed from tour.",
            [MessageCode.GreedyFinish] = "Greedy algorithm finished successfully.",
            [MessageCode.HDIStarting] = "Starting \"Hybrid Distance Insertion\" improvement...",
            [MessageCode.HDUStarting] = "Starting \"Hybrid Distance Update\" improvement...",
            [MessageCode.HDINewNodeAdded] = "Point of interest \"{0}\" added successfully to the Tour.",
            [MessageCode.HDINewNodeRemoved] = "Point of interest \"{0}\" removed from the Tour.",
            [MessageCode.HDUTourUpdated] = "Point of interest \"{0}\" replaced with point of interest \"{1}\".",
            [MessageCode.HDUTourRestored] = "Point of interest \"{0}\" removed. Tour restored with \"{1}\".",
            [MessageCode.LSStartSolution] = "Starting with solution {0} with total cost {1}.",
            [MessageCode.LSResumeSolution] = "Resume Local Search with solution {0} with total cost {1}.",
            [MessageCode.LSNewNeighborhood] = "Generating new neighborhood {0}.",
            [MessageCode.LSNewNeighborhoodMove] = "Adding new solution {0} to neighborhood {1}.",
            [MessageCode.LSNewNeighborhoodMoveDetails] = "New solution {0} details: swapping between edge {1} and edge {2}.",
            [MessageCode.LSNeighborhoodBest] = "The best move for this neighborhood is \"{0}\" with total cost: {1}.",
            [MessageCode.LSBestFound] = "Best solution found with total cost: ({0}). Previous solution total cost: ({1}).",
            [MessageCode.LSInvariateSolution] = "For this neighborhood a better solution of current best (id: {0} , total cost: {1}), hasn't found...",
            [MessageCode.LSFinish] = "LS Stop condition occurred with solution {0} with total cost: ({1})",
            [MessageCode.LKStartSolution] = "Starting with current LS Best Solution {0} with cost {1}.",
            [MessageCode.LKHStepIncreased] = "Lin Kernighan Step {0} of {1}.",
            [MessageCode.LKStarting] = "Max iterations with no improvements reached. Lin Kernighan starting...",
            [MessageCode.LKBlockedMove] = "Move {0} already selected! Continue searching...",
            [MessageCode.LKNoSNodeSelected] = "No one move can be selected! Terminating...",
            [MessageCode.LKBestFound] = "Best solution found with total cost: ({0}).",
            [MessageCode.LKInvariateSolution] = "A better solution of current best (id: {0} , total cost: {1}), hasn't found. Terminating...",
            [MessageCode.LKFinish] = "Lin Kernighan algorithm finished successfully.",
            [MessageCode.TSStarting] = "Tabu Search starts with solution {0} with total cost {1}",
            [MessageCode.TSBestFound] = "Tabu Search has found a new best solution found with total cost: ({0}). Previous solution total cost: ({1}).",
            [MessageCode.TSMoveLocked] = "Move -> First edge: {0} - Second edge: {1} added in Tabu List. Move locked.",
            [MessageCode.TSMovesLocked] = "Tabu List status -> First edge: {0} - Second edge: {1} Expiration: {2}, Tenure: {3}",
            [MessageCode.TSMoveUnlocked] = "Reverse move unlocked -> First edge: {0} - Second edge: {1} removed from Tabu List. Expiration {2}",
            [MessageCode.ALCompletionSummary] = "Execution summary:",
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
            [MessageCode.TOSolutionCollectionId] = "Solution {0}:",
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
      GreedyFinish,
      GreedyNodeAdded,
      GreedyNodeRemoved,
      HDIStarting,
      HDUStarting,
      HDINewNodeAdded,
      HDINewNodeRemoved,
      HDUTourUpdated,
      HDUTourRestored,
      LSStartSolution,
      LSResumeSolution,
      LSNewNeighborhood,
      LSNewNeighborhoodMove,
      LSNewNeighborhoodMoveDetails,
      LSNeighborhoodBest,
      LSBestFound,
      LSInvariateSolution,
      LSFinish,
      LKStartSolution,
      LKHStepIncreased,
      LKStarting,
      LKBlockedMove,
      LKNoSNodeSelected,
      LKBestFound,
      LKInvariateSolution,
      LKFinish,
      TSStarting,
      TSBestFound,
      TSMoveLocked,
      TSMovesLocked,
      TSMoveUnlocked,
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
