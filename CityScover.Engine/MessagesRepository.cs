﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 11/11/2018
//

using System.Collections.Generic;

namespace CityScover.Engine.Algorithms
{
   internal static class MessagesRepository
   {
      private static IDictionary<MessageCode, string> _algorithmMessages;

      #region Static constructors
      static MessagesRepository()
      {
         _algorithmMessages = new Dictionary<MessageCode, string>()
         {
            [MessageCode.None] = string.Empty,
            [MessageCode.GreedyNodeAdded] = "Point of interest \"{0}\" added to solution {1}.",
            [MessageCode.GreedyNodeRemoved] = "Point of interest \"{0}\" removed from tour.",
            [MessageCode.GreedyFinish] = "Greedy algorithm finished successfully.",
            [MessageCode.CustomAlgStart] = "Starting Hybrid Nearest Distance improvement algorithm...",
            [MessageCode.CustomAlgNodeAdded] = "Point of interest \"{0}\" added successfully to the Tour.",
            [MessageCode.CustomAlgNodeRemoved] = "Point of interest \"{0}\" removed from the Tour.",
            [MessageCode.LSStartSolution] = "Starting with solution {0} with cost {1}.",
            [MessageCode.LSNewNeighborhood] = "Generating new neighborhood {0}.",
            [MessageCode.LSNewNeighborhoodMove] = "Adding new solution {0} to neighborhood {1}.",
            [MessageCode.LSNewNeighborhoodMoveDetails] = "New solution {0} details: changed edge {1} with edge {2}.",
            [MessageCode.LSNeighborhoodBest] = "The best move for this neighborhood is: {0} with cost: {1}.",
            [MessageCode.LSBestFound] = "Best solution found with cost: ({0}). Previous solution cost: ({1}).",
            [MessageCode.LSFinish] = "Stop condition occurred with solution cost: ({0})",
            [MessageCode.LKHStepIncreased] = "Step {0} of {1} with no improvement.",
            [MessageCode.LKStarting] = "Max iterations with no improvements reached. Lin Kernighan starting...",
            [MessageCode.LKBestFound] = "Best solution found with cost: ({0}).",
            [MessageCode.OnCompletedHeader] = "Solutions produced for stage \"{0}\" are:",
            [MessageCode.EXREPSolutionReceived] = "Solution {0} has been marked with cost: {1}",
            [MessageCode.EXREPSolutionReceivedWithPenalty] = "Solution {0} has been marked with cost: {1} and penalty: {2}",
            [MessageCode.EXREPAlgorithmPerformance] = "The algorithm {0} performed in {1}.",
            [MessageCode.EXREPExceptionOccurred] = "{0}: Exception occurred: {1}.",
            [MessageCode.EXREPTimeFormat] = "{0} Hours, {1} Minutes, {2} Seconds.",
            [MessageCode.ViolatedConstraints] = "Violated Constraints detail",
            [MessageCode.ViolatedConstraint] = "{0} --> VIOLATED! ",
            [MessageCode.SolverStageStart] = "Starting Stage \"{0}\"...",
            [MessageCode.SolverStageEnd] = "Stage \"{0}\" completed.",
            [MessageCode.SolverGraphCreationError] = "Empty graph. Error during creation of the graph in method \"{0}\"."
         };
      }
      #endregion

      #region Internal methods
      internal static string GetMessage(MessageCode code, params object[] messageList)
      {
         if (!_algorithmMessages.ContainsKey(code))
         {
            return string.Empty;
         }

         return string.Format(_algorithmMessages[code], messageList);
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
        CustomAlgStart,
        CustomAlgNodeAdded,
        CustomAlgNodeRemoved,
        LSStartSolution,
        LSNewNeighborhood,
        LSNewNeighborhoodMove,
        LSNewNeighborhoodMoveDetails,
        LSNeighborhoodBest,
        LSBestFound,
        LSFinish,
        LKHStepIncreased,
        LKStarting,
        LKBestFound,
        OnCompletedHeader,
        EXREPSolutionReceived,
        EXREPSolutionReceivedWithPenalty,
        EXREPAlgorithmPerformance,
        EXREPExceptionOccurred,
        EXREPTimeFormat,
        ViolatedConstraints,
        ViolatedConstraint,
        SolverStageStart,
        SolverStageEnd,
        SolverGraphCreationError
    }
    #endregion
}
