//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 13/10/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// This class acts as a mediator between the Algorithm in execution and observer objects.
   /// His role is to notify all observer that a new solution has been produced.
   /// </summary>
   internal class AlgorithmTracker : IObservable<TOSolution>
   {
      #region Private fields
      private ICollection<IObserver<TOSolution>> _observers;
      #endregion

      #region Constructors
      public AlgorithmTracker()
      {
         _observers = new Collection<IObserver<TOSolution>>();
      }
      #endregion

      #region Public methods
      public virtual void NotifyObservers(TOSolution solution)
      {
         foreach (var observer in _observers)
         {
            observer.OnNext(solution);
         }
      }

      public virtual void NotifyError(Exception exception)
      {
         foreach (var observer in _observers)
         {
            observer.OnError(exception);
         }
      }

      public virtual void NotifyCompletion()
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
