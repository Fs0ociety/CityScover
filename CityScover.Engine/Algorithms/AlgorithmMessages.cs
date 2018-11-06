//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 06/11/2018
//

using System.Collections.Generic;

namespace CityScover.Engine.Algorithms
{
   internal static class AlgorithmMessages
   {
      private static IDictionary<MessageCodes, string> _algorithmMessages;

      #region Static constructors
      static AlgorithmMessages()
      {
         _algorithmMessages = new Dictionary<MessageCodes, string>()
         {
            [MessageCodes.None] = string.Empty,
            [MessageCodes.BestSolutionFound] = "Best solution found.",
            [MessageCodes.NewSolutionComponentAdded] = "Point id {0} added to solution."
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
      BestSolutionFound = 1,
      NewSolutionComponentAdded = 2
   }
}
