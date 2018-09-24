//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/09/2018
//

using System;

namespace CityScover.Engine
{
   /// <summary>
   /// This abstract class represents a generic interface Algorithm to execute.
   /// </summary>
   internal abstract class Algorithm
   {
      private bool _acceptImprovements;
      private AlgorithmTracker _provider;

      #region Protected members
      protected AlgorithmStatus _status;
      protected ushort _currentStep = default;
      protected Action<TOSolution> notifyingFunc = default;
      protected Solver Solver => Solver.Instance;
      #endregion

      #region Constructors
      internal Algorithm()
         : this(null)
      {
      }

      internal Algorithm(AlgorithmTracker provider, bool acceptImprovements = true)
      {
         _acceptImprovements = acceptImprovements;
         _provider = provider;
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
         get => _acceptImprovements;
         set
         {
            if (_acceptImprovements != value)
            {
               _acceptImprovements = value;
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

      #region Internal methods
      internal void Start()
      {         
         OnInitializing();

         _status = AlgorithmStatus.Running;

         while (!StopConditions())
         {
            try
            {
               PerformStep();
               _currentStep++;
            }
            catch
            {               
               OnError();
            }
         }
                  
         OnTerminating();
         
         OnTerminated();
      }
      #endregion

      #region Internal abstract methods
      internal abstract void PerformStep();
      internal abstract bool StopConditions();
      #endregion

      #region Virtual methods
      internal virtual void OnInitializing()
      {
         if (Solver.IsMonitoringEnabled)
         {
            notifyingFunc = _provider.NotifyObservers;
         }
         else
         {
            notifyingFunc = Solver.ValidatingQueue.Add;
         }
         _status = AlgorithmStatus.Initializing;
      }
      internal virtual void OnTerminating()
      {
         _status = AlgorithmStatus.Terminating;
      }

      internal virtual void OnTerminated()
      {
         if (Solver.IsMonitoringEnabled)
         {
            _provider.NotifyCompletion();
         }
         _status = AlgorithmStatus.Terminated;
      }

      internal virtual void OnError()
      {
         if (Solver.IsMonitoringEnabled)
         {
            _provider.NotifyError();
         }
         _status = AlgorithmStatus.Error;
      }
      #endregion
   }
}