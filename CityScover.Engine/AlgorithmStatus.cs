//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/11/2018
//

namespace CityScover.Engine
{
   #region AlgorithmStatus enumeration
   internal enum AlgorithmStatus
   {
      /// <summary>
      /// Invalid status of the Algorithm.
      /// </summary>
      None,

      /// <summary>
      /// Phase of initialization before the Algorithm starts its execution.
      /// </summary>
      Initializing,

      /// <summary>
      /// Running's phase of the Algorithm.
      /// </summary>
      Running,

      /// <summary>
      /// The Algorithm is starting its ending phase.
      /// </summary>
      Terminating,

      /// <summary>
      /// The Algorithm has terminated its execution.
      /// </summary>
      Terminated,

      /// <summary>
      /// Error status of the Algorithm.
      /// </summary>
      Error
   }
   #endregion
}