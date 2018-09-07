//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 07/09/2018
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
   internal class ExecutionReporter : Singleton<ExecutionReporter>, IObserver<Solution>
   {
      #region Constructors
      private ExecutionReporter()
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

      #region Interfaces implementation
      public void OnNext(Solution value)
      {
         throw new NotImplementedException();
      }

      public void OnError(Exception error)
      {
         throw new NotImplementedException();
      }

      public void OnCompleted()
      {
         throw new NotImplementedException();
      }
      #endregion
   }
}
