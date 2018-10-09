//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/10/2018
//

using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// This abstract class represents the interface for a generic Algorithm type.
   /// </summary>
   internal abstract class Algorithm
   {
      private bool _acceptImprovementsOnly;
      private AlgorithmTracker _provider;

      #region Protected members
      protected AlgorithmStatus _status;
      protected ushort _currentStep;
      protected Solver Solver => Solver.Instance;
      #endregion

      #region Constructors
      internal Algorithm()
         : this(null)
      {
      }

      internal Algorithm(AlgorithmTracker provider, bool acceptImprovementsOnly = true)
      {
         _acceptImprovementsOnly = acceptImprovementsOnly;
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

      #region Internal methods
      internal async Task Start()
      {         
         OnInitializing();

         _status = AlgorithmStatus.Running;

         while (!StopConditions())
         {
            try
            {
               await Task.Run(() => PerformStep());
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
      internal abstract Task PerformStep();
      internal abstract bool StopConditions();
      #endregion

      #region Virtual methods
      internal virtual void OnInitializing()
      {
         _status = AlgorithmStatus.Initializing;
      }
      internal virtual void OnTerminating()
      {
         _status = AlgorithmStatus.Terminating;
      }

      internal virtual void OnTerminated()
      {
         _status = AlgorithmStatus.Terminated;
         if (Solver.IsMonitoringEnabled)
         {
            _provider.NotifyCompletion();
         }
      }

      internal virtual void OnError()
      {
         _status = AlgorithmStatus.Error;
         if (Solver.IsMonitoringEnabled)
         {
            _provider.NotifyError();
         }
      }
      #endregion
   }
}