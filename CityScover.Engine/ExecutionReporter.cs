//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 08/09/2018
//

using System;
using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represents the object involved to create the right Algorithm 
   /// and start execution of all stages of the Algorithm.
   /// In addition, it monitors the execution of the Algorithm.
   /// </summary>
   internal class ExecutionReporter : IObserver<BaseSolution>
   {
      private IDisposable _unsubscriber;

      #region Constructors
      public ExecutionReporter()
      {
         // TODO
      }
      #endregion

      #region Internal properties
      // TODO
      #endregion

      #region Internal methods
      /// <summary>
      /// Set its own provider passed as parameter to receive notifications 
      /// and gets the unsubscriber object to cancel subscription.
      /// </summary>
      /// <param name="provider"></param>
      internal virtual void Subscribe(AlgorithmTracker provider) => _unsubscriber = provider.Subscribe(this);

      /// <summary>
      /// Cancel the subscription to provider.
      /// </summary>
      internal virtual void Unsubscribe() => _unsubscriber.Dispose();

      /// <summary>
      /// Invoke the algorithm passed as argument and reports its running time.
      /// </summary>
      /// <param name="algorithm">Algorithm to execute.</param>
      /// <returns></returns>
      internal async Task Run(Algorithm algorithm)
      {
         if (algorithm == null)
         {
            throw new ArgumentNullException(nameof(algorithm));
         }

         // TODO: Algorithm's invocation logic.
         throw new NotImplementedException();
      }
      #endregion

      #region Interfaces implementation
      public void OnNext(BaseSolution value)
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
