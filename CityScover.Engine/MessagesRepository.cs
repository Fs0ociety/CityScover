//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 07/11/2018
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
            [MessageCodes.GreedyFinish] = "Greedy algorithm finished successfully.",
            [MessageCodes.NNPointAdded] = "Point of interest \"{0}\" added to solution.",
            [MessageCodes.CustomAlgNodeAdded] = "Point of interest \"{0}\" added successfully to the Tour.",
            [MessageCodes.CustomAlgNodeRemoved] = "Point of interest \"{0}\" removed from the Tour.",
            [MessageCodes.LSBestFound] = "Best solution found with cost: ({0}). Previous solution cost: ({1}).",
            [MessageCodes.LSFinish] = "Stop condition occurred with solution cost: ({0})",
            [MessageCodes.LKHStepIncreased] = "Step {0} of {1} with no improvement.",
            [MessageCodes.LKStarting] = "Max iterations with no improvements reached. Lin Kernighan starting...",
            [MessageCodes.LKBestFound] = "Best solution found with cost: ({0}).",
            [MessageCodes.OnCompletedHeader] = "Solution computed for stage \"{0}\" is "
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
      GreedyFinish = 1,
      NNPointAdded = 2,
      CustomAlgNodeAdded = 3,
      CustomAlgNodeRemoved = 4,
      LSBestFound = 5,
      LSFinish = 6,
      LKHStepIncreased = 7,
      LKStarting = 8,
      LKBestFound = 9,
      OnCompletedHeader = 10
   }
}
