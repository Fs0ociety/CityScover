//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/09/2018
//

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

      #region Internal abstract methods
      internal abstract void OnInitializing();
      internal abstract void PerformStep();
      internal abstract void OnTerminating();
      internal abstract void OnTerminated();
      internal abstract void OnError();
      internal abstract bool StopConditions();
      #endregion
   }
}