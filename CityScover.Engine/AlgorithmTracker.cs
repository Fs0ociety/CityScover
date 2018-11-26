//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 26/11/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine
{
   /// <summary>
   /// This class acts as a mediator between the Algorithm in execution and observer objects.
   /// His role is to notify all observer that a new solution has been produced.
   /// </summary>
   internal class AlgorithmTracker : IObservable<TOSolution>
   {
      #region Private fields
      private readonly ICollection<IObserver<TOSolution>> _observers;
      #endregion

      #region Constructors
      public AlgorithmTracker()
      {
         _observers = new Collection<IObserver<TOSolution>>();
      }
      #endregion

      #region Public methods
      internal void NotifyObservers(TOSolution solution)
      {
         foreach (var observer in _observers)
         {
            observer.OnNext(solution);
         }
      }

      internal void NotifyObservers(string message)
      {
         if (message is null)
         {
            NotifyError(new ArgumentNullException(nameof(message)));
         }

         var executionReporter = _observers.FirstOrDefault();
         if (executionReporter is null)
         {
            NotifyError(new NullReferenceException());
         }

         ((ExecutionReporter)executionReporter).OnNextMessage(message);
      }

      internal void NotifyError(Exception exception)
      {
         foreach (var observer in _observers)
         {
            observer.OnError(exception);
         }
      }

      internal void NotifyCompletion()
      {
         foreach (var observer in _observers)
         {
            observer.OnCompleted();
         }
      }
      #endregion

      #region Interfaces implementation
      public IDisposable Subscribe(IObserver<TOSolution> observer)
      {
         // Check whether observer is already registered. If not, add it.
         if (!_observers.Contains(observer))
         {
            _observers.Add(observer);
         }
         return new Unsubscriber<TOSolution>(_observers, observer);
      }
      #endregion
   }
}
