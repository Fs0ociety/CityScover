//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 07/09/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal class AlgorithmTracker : IObservable<Solution>
   {
      private ICollection<IObserver<Solution>> _observers;

      #region Constructors
      public AlgorithmTracker()
      {
         _observers = new Collection<IObserver<Solution>>();
      }
      #endregion

      #region Public methods
      public virtual void NotifyObservers(Solution solution)
      {
         foreach (var observer in _observers)
         {
            observer.OnNext(solution);
         }
      }

      public virtual void NotifyError()
      {
         foreach (var observer in _observers)
         {
            observer.OnError(new Exception("Cost exceeds maximum value."));
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
      public IDisposable Subscribe(IObserver<Solution> observer)
      {
         // Check whether observer is already registered. If not, add it.
         if (!_observers.Contains(observer))
         {
            _observers.Add(observer);
         }
         return new Unsubscriber<Solution>(_observers, observer);
      }
      #endregion
   }
}
