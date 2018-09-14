//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 14/09/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal class AlgorithmTracker : IObservable<TOSolution>
   {
      private ICollection<IObserver<TOSolution>> _observers;

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
