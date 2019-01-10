//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 10/01/2019
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
            [MessageCode.GreedyNotFoundValidSolutions] = "{0} has not found valid solutions. EXECUTION SUMMARY not available.",
            [MessageCode.GreedyStop] = "Greedy algorithm finished successfully.",

            [MessageCode.LocalSearchStartSolution] = "Local Search starting with solution {0} with cost: {1}",
            [MessageCode.LocalSearchResumeSolution] = "Resume Local Search with solution {0} with total cost {1}.",
            [MessageCode.LocalSearchNewNeighborhood] = "Generating new neighborhood {0}.",
            [MessageCode.LocalSearchNewNeighborhoodMove] = "Adding new solution {0} to neighborhood {1}.",
            [MessageCode.LocalSearchNewNeighborhoodMoveDetails] = "New solution {0} details: swapping between edge {1} and edge {2}.\nSwap produces new edges {3} and {4}.",
            [MessageCode.LocalSearchNeighborhoodBest] = "The best move for this neighborhood is \"{0}\" with cost --> {1}.",
            [MessageCode.LocalSearchNeighborhoodBestUnderThreshold] = "The best neighborhood move {0} of cost {1} isn't so good. Not considered an improvement.",
            [MessageCode.LocalSearchBestFound] = "Best solution found with COST: {0}. Previous solution COST: {1}.",
            [MessageCode.LocalSearchInvariateSolution] = "For this neighborhood a better solution than current best hasn't found. Current best solution --> ID: {0}, COST: {1}.",
            [MessageCode.LocalSearchImprovementsPerformed] = "Local Search has performed a total of {0} improvements.",
            [MessageCode.LocalSearchStop] = "Local Search has not found a better solution and it stops with current solution ID: {0} and COST: {1}",

            [MessageCode.HybridCustomInsertionStart] = "Hybrid Custom Insertion improvement algorithm starts...",
            [MessageCode.HybridCustomInsertionNewNodeAdded] = "Point of interest \"{0}\" added successfully to the tour.",
            [MessageCode.HybridCustomInsertionNewNodeRemoved] = "Point of interest \"{0}\" removed from the Tour.",
            [MessageCode.HybridCustomInsertionStopWithSolution] = "Hybrid Custom Insertion STOP condition occurred with solution ID: {0} and COST: {1}",
            [MessageCode.HybridCustomInsertionTourUpdated] = "Hybrid Custom Update algorithm has finished with the tour UPDATED!",
            [MessageCode.HybridCustomInsertionTourNotUpdated] = "Hybrid Custom Update algorithm has finished with the tour NOT UPDATED!",
            [MessageCode.HybridCustomInsertionFinalSolution] = "HCI has found a new best solution! Solver's best solution updated with solution ID: {0} and COST: {1}. " +
                                                               "Solver's previous best solution ID: {2} with COST: {3}",

            [MessageCode.HybridCustomUpdateStart] = "Hybrid Custom Update improvement algorithm starts...",
            [MessageCode.HybridCustomUpdateTourUpdated] = "Point of interest \"{0}\" replaced with point of interest \"{1}\".",
            [MessageCode.HybridCustomUpdateTourRestored] = "Point of interest \"{0}\" removed. Tour restored with \"{1}\".",
            [MessageCode.HybridCustomUpdatePointsReplaced] = "Point ID {0} removed from the tour and Point ID {1} added to the tour.",
            [MessageCode.HybridCustomUpdateStopWithSolution] = "Hybrid Custom Update STOP condition occurred with solution {0} with total cost --> {1}",
            [MessageCode.HybridCustomUpdateStopWithoutSolution] = "Hybrid Custom Update STOP condition occurred immediately. Tour not updated! Exit.",
            [MessageCode.HybridCustomUpdateBestFinalTour] = "HCU has found a new BEST solution! Solver's best solution updated with solution ID: {0} and COST: {1}. " +
                                                            "Solver's previous best solution ID: {2} with COST: {3}",
            [MessageCode.HybridCustomUpdateWorseFinalTour] = "HCU has found a new WORSE solution! Solver's best solution updated with solution ID: {0} and COST: {1}. " +
                                                            "Solver's previous best solution ID: {2} with COST: {3}",
            [MessageCode.HybridCustomUpdateUnchangedTour] = "HCU has found a new solution with cost equals to previous Solver's best solution.",

            [MessageCode.LinKernighanStartSolution] = "Lin Kernighan starting with current Local Search Best Solution ID: {0} and COST: {1}.",
            [MessageCode.LinKernighanHStepIncreased] = "Lin Kernighan Step {0} of {1}.",
            [MessageCode.LinKernighanMoveDetails] = "New solution {0} details: removed edge {1}.",
            [MessageCode.LinKernighanStart] = "Max iterations with no improvements reached. Starting Lin Kernighan...",
            [MessageCode.LinKernighanBlockedMove] = "Move {0} already selected! Continue searching...",
            [MessageCode.LinKernighanNoSNodeSelected] = "No one move can be selected! Terminating...",
            [MessageCode.LinKernighanBestFound] = "Best solution found with total cost: ({0}).",
            [MessageCode.LinKernighanInvariateSolution] = "A better solution than current best (ID: {0}, COST: {1}), hasn't found. Terminating...",
            [MessageCode.LinKernighanStop] = "Lin Kernighan algorithm finished successfully.",

            [MessageCode.TabuSearchStart] = "Tabu Search starts with solution {0} with cost: {1}",
            [MessageCode.TabuSearchBestFound] = "Tabu Search has found a new best solution with COST: {0}. Previous solution COST: {1}.",
            [MessageCode.TabuSearchMoveLocked] = "Move -> First edge: {0} - Second edge: {1} added in Tabu List. Move locked.",
            [MessageCode.TabuSearchMovesLocked] = "Tabu List status -> First edge: {0} - Second edge: {1} Expiration: {2}, Tenure: {3}",
            [MessageCode.TabuSearchMoveUnlocked] = "Reverse move unlocked -> First edge: {0} - Second edge: {1} removed from Tabu List. Expiration {2}",
            [MessageCode.TabuSearchImprovementsPerformed] = "Tabu Search has performed a total of {0} improvements.",
            [MessageCode.TabuSearchStop] = "Tabu Search STOP condition occurred with solution ID: {0} and COST: {1}",

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
            [MessageCode.SolverUpdatedSolution] = "The Solver's Best Solution is SOLUTION ID: {0} and COST: {1}",
            [MessageCode.SolverGraphCreationError] = "Empty graph. Error during creation of the graph in method \"{0}\".",
            [MessageCode.TOSolutionCollectionId] = "Solution {0}\n",
            [MessageCode.CMGraphNodeToString] = "{0} at {1}",
            [MessageCode.TOSolutionFinalTour] = "The final best tour is solution \"{0}\":\n{1}",
            [MessageCode.TOSolutionTotalTimeAndValidity] = "Cost: {0}. Tour time: {1} hours and {2} minutes. Admissible: {3}.",
            [MessageCode.TOSolutionTotalDistance] = "Tour distance: {0:0.##} kilometers."
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
      GreedyNotFoundValidSolutions,
      GreedyStop,
      HybridCustomInsertionStart,
      HybridCustomInsertionNewNodeAdded,
      HybridCustomInsertionNewNodeRemoved,
      HybridCustomInsertionFinalSolution,
      HybridCustomInsertionStopWithSolution,
      HybridCustomInsertionTourUpdated,
      HybridCustomInsertionTourNotUpdated,
      HybridCustomUpdateStart,
      HybridCustomUpdateTourUpdated,
      HybridCustomUpdateTourRestored,
      HybridCustomUpdateBestFinalTour,
      HybridCustomUpdateWorseFinalTour,
      HybridCustomUpdateUnchangedTour,
      HybridCustomUpdatePointsReplaced,
      HybridCustomUpdateStopWithSolution,
      HybridCustomUpdateStopWithoutSolution,
      LocalSearchStartSolution,
      LocalSearchResumeSolution,
      LocalSearchNewNeighborhood,
      LocalSearchNewNeighborhoodMove,
      LocalSearchNeighborhoodBestUnderThreshold,
      LocalSearchNewNeighborhoodMoveDetails,
      LocalSearchNeighborhoodBest,
      LocalSearchBestFound,
      LocalSearchInvariateSolution,
      LocalSearchImprovementsPerformed,
      LocalSearchStop,
      LinKernighanStartSolution,
      LinKernighanHStepIncreased,
      LinKernighanStart,
      LinKernighanMoveDetails,
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
      SolverUpdatedSolution,
      SolverGraphCreationError,
      TOSolutionCollectionId,
      CMGraphNodeToString,
      TOSolutionFinalTour,
      TOSolutionTotalTimeAndValidity,
      TOSolutionTotalDistance
   }
   #endregion
}
