//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/10/2018
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
            [MessageCodes.BestSolutionFound] = "Best solution found"
         };
      }
      #endregion

      #region Internal methods
      internal static string GetMessage(MessageCodes code, params string[] messageList)
      {
         if (!_algorithmMessages.ContainsKey(code))
         {
            return string.Empty;
         }

         return _algorithmMessages[code];
      }
      #endregion
   }

   internal enum MessageCodes
   {
      None = 0,
      BestSolutionFound = 1,
   }
}
