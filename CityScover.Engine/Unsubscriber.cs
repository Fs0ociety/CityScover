//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/11/2018
//

using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   /// <typeparam name="TSolution"></typeparam>
   internal class Unsubscriber<TSolution> : IDisposable
   {
      #region Private fields
      private readonly ICollection<IObserver<TSolution>> _observers;
      private readonly IObserver<TSolution> _observer;
      #endregion

      #region Constructors
      internal Unsubscriber(ICollection<IObserver<TSolution>> observers, IObserver<TSolution> observer)
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