//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 25/08/2018
//

using CityScover.Utils;
using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represents the object involved to create the right Algorithm 
   /// and start execution of all stages of the Algorithm.
   /// In addition, it monitors the execution of the Algorithm.
   /// </summary>
   internal class ExecutionTracer : Singleton<ExecutionTracer>
   {
      #region Constructors
      private ExecutionTracer()
      {
      }
      #endregion

      #region Private methods
      private Algorithm CreateAlgorithm(AlgorithmType currentAlgorithm)
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Public methods
      public void Run(IEnumerable<Stage> stages)
      {
         throw new NotImplementedException();
      }
      #endregion
   }
}
