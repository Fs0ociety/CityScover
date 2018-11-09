//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 08/11/2018
//

using System.Collections.Generic;

namespace CityScover.Engine.Algorithms
{
   internal static class MessagesRepository
   {
      private static IDictionary<MessageCodes, string> _algorithmMessages;

      #region Static constructors
      static MessagesRepository()
      {
         _algorithmMessages = new Dictionary<MessageCodes, string>()
         {
            [MessageCodes.None] = string.Empty,
            [MessageCodes.StageStart] = "Starting Stage {0}...",
            [MessageCodes.GreedyNodeAdded] = "Point of interest \"{0}\" added to solution {1}.",
            [MessageCodes.GreedyFinish] = "Greedy algorithm finished successfully.",
            [MessageCodes.CustomAlgStart] = "Starting Hybrid Nearest Distance improvement algorithm...",
            [MessageCodes.CustomAlgNodeAdded] = "Point of interest \"{0}\" added successfully to the Tour.",
            [MessageCodes.CustomAlgNodeRemoved] = "Point of interest \"{0}\" removed from the Tour.",
            [MessageCodes.LSNewNeighborhood] = "Generating new neighborhood {0}.",
            [MessageCodes.LSNewNeighborhoodMove] = "Adding new solution {0} to neighborhood {1}.",
            [MessageCodes.LSNewNeighborhoodMoveDetails] = "New solution {0} details: changed edge {1} with edge {2}.",
            [MessageCodes.LSNeighborhoodBest] = "The best move for this neighborhood is: {0} with cost: {1}.",
            [MessageCodes.LSBestFound] = "Best solution found with cost: ({0}). Previous solution cost: ({1}).",
            [MessageCodes.LSFinish] = "Stop condition occurred with solution cost: ({0})",
            [MessageCodes.LKHStepIncreased] = "Step {0} of {1} with no improvement.",
            [MessageCodes.LKStarting] = "Max iterations with no improvements reached. Lin Kernighan starting...",
            [MessageCodes.LKBestFound] = "Best solution found with cost: ({0}).",
            [MessageCodes.OnCompletedHeader] = "Solutions produced for stage \"{0}\" are:",
            [MessageCodes.EXREPSolutionReceived] = "Solution {0} has been marked with cost: {1}",
            [MessageCodes.EXREPSolutionReceivedWithPenalty] = "Solution {0} has been marked with cost: {1} and penalty: {2}",
            [MessageCodes.EXREPAlgorithmPerformance] = "The algorithm {0} performed in {1}.",
            [MessageCodes.EXREPExceptionOccurred] = "{0}: Exception occurred: {1}.",
            [MessageCodes.EXREPTimeFormat] = "{0} Hours, {1} Minutes, {2} Seconds.",
            [MessageCodes.ViolatedConstraints] = "Violated Constraints detail",
            [MessageCodes.ViolatedConstraint] = "{0} = violated"
         };
      }
      #endregion

      #region Internal methods
      internal static string GetMessage(MessageCodes code, params object[] messageList)
      {
         if (!_algorithmMessages.ContainsKey(code))
         {
            return string.Empty;
         }

         return string.Format(_algorithmMessages[code], messageList);
      }
      #endregion
   }

   internal enum MessageCodes
   {
      None = 0,
      StageStart = 1,
      GreedyFinish = 2,
      GreedyNodeAdded = 3,
      CustomAlgStart = 4,
      CustomAlgNodeAdded = 5,
      CustomAlgNodeRemoved = 6,
      LSNewNeighborhood = 7,
      LSNewNeighborhoodMove = 8,
      LSNewNeighborhoodMoveDetails = 9,
      LSNeighborhoodBest = 10,
      LSBestFound = 11,
      LSFinish = 12,
      LKHStepIncreased = 13,
      LKStarting = 14,
      LKBestFound = 15,
      OnCompletedHeader = 16,
      EXREPSolutionReceived = 17,
      EXREPSolutionReceivedWithPenalty = 18,
      EXREPAlgorithmPerformance = 19,
      EXREPExceptionOccurred = 20,
      EXREPTimeFormat = 21,
      ViolatedConstraints = 22,
      ViolatedConstraint = 23
   }
}
