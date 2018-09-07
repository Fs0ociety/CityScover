//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 07/09/2018
//

using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   /// <typeparam name="Solution"></typeparam>
   internal class Unsubscriber<Solution> : IDisposable
   {
      private ICollection<IObserver<Solution>> _observers;
      private readonly IObserver<Solution> _observer;

      #region Constructors
      internal Unsubscriber(ICollection<IObserver<Solution>> observers, IObserver<Solution> observer)
      {
         _observers = observers;
         _observer = observer;
      }
      #endregion

      #region Interfaces implementation
      public void Dispose()
      {
         if (_observers.Contains(_observer))
            _observers.Remove(_observer);
      }
      #endregion
   }
}