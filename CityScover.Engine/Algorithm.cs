//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 15/09/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine
{
   /// <summary>
   /// This abstract class represents a generic interface Algorithm to execute.
   /// </summary>
   internal abstract class Algorithm
   {
      private ushort _currentStep;
      private AlgorithmStatus _status;
      private bool _acceptImprovementsOnly;
      private AlgorithmTracker _provider;
      private ICollection<AlgorithmType> _innerAlgorithmTypes;
      private ICollection<Algorithm> _innerAlgorithms;

      #region Constructors
      internal Algorithm()
         : this(null)
      { }

      internal Algorithm(AlgorithmTracker provider)
      {
         _acceptImprovementsOnly = true;
         _provider = provider;
         _innerAlgorithms = new Collection<Algorithm>();
      }
      #endregion

      #region Internal properties
      protected ushort CurrentStep
      {
         get => _currentStep;
         set
         {
            if (_currentStep != value)
            {
               _currentStep = value;
            }
         }
      }
      internal AlgorithmStatus Status
      {
         get => _status;
         private set
         {
            if (_status != value)
            {
               _status = value;
            }
         }
      }

      internal bool AcceptImprovementsOnly
      {
         get => _acceptImprovementsOnly;
         set
         {
            if (_acceptImprovementsOnly != value)
            {
               _acceptImprovementsOnly = value;
            }
         }
      }

      internal AlgorithmTracker Provider
      {
         get => _provider;
         set
         {
            if (_provider != value)
            {
               _provider = value;
            }
         }
      }
      #endregion

      #region Internal abstract methods
      internal abstract void OnInitializing();
      internal abstract void PerformStep();
      internal abstract void OnTerminating();
      internal abstract void OnTerminated();
      internal abstract void OnError();
      internal abstract bool StopConditions();
      internal void Start()
      {
         _status = AlgorithmStatus.Initializing;
         OnInitializing();
         // TODO: Pubblicazione dell'evento di inizializazione ai subscribers.

         _status = AlgorithmStatus.Running;
         // TODO: Pubblicazione dell'evento di esecuzione ai subscribers.

         while (!StopConditions())
         {
            try
            {
               PerformStep();
               _currentStep++;
            }
            catch
            {
               _status = AlgorithmStatus.Error;
               OnError();
               // TODO: Pubblicazione dell'evento di errore ai subscribers.
            }
         }

         _status = AlgorithmStatus.Terminating;
         OnTerminating();
         // TODO: Pubblicazione dell'evento di inizio terminazione ai subscribers.

         _status = AlgorithmStatus.Terminated;
         OnTerminated();
         // TODO: Pubblicazione dell'evento di avvenuta terminazione ai subscribers.
      }
      #endregion

      #region Protected methods
      protected AlgorithmType GetInnerAlgorithmType(AlgorithmType algorithmType)
      {
         return (from type in _innerAlgorithmTypes
                where algorithmType == type
                select type).FirstOrDefault();
      }
      #endregion
   }
}
