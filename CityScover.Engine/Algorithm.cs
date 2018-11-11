//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 11/11/2018
//

using CityScover.Engine.Algorithms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// This abstract class represents the interface for a generic Algorithm type.
   /// </summary>
   internal abstract class Algorithm
   {
      #region Private members
      private AlgorithmStatus _status;
      private ushort _currentStep;
      private bool _acceptImprovementsOnly;
      private AlgorithmTracker _provider;
      #endregion

      #region Constructors
      internal Algorithm()
         : this(null)
      {
      }

      internal Algorithm(AlgorithmTracker provider, bool acceptImprovementsOnly = true)
      {
         CurrentAlgorithm = Solver.CurrentStage.Flow.CurrentAlgorithm;
         _acceptImprovementsOnly = acceptImprovementsOnly;
         _provider = provider;
      }
      #endregion

      #region Private Protected properties
      private protected Solver Solver => Solver.Instance;
      private protected AlgorithmType CurrentAlgorithm { get; set; }

      private protected AlgorithmStatus Status
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
      #endregion

      #region Internal properties
      internal ushort CurrentStep
      {
         get => _currentStep;
         private protected set
         {
            if (_currentStep != value)
            {
               _currentStep = value;
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

      internal IDictionary<ParameterCodes, dynamic> Parameters { get; set; }
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
            catch(Exception ex)
            {
               OnError(ex);
            }
         }
                  
         OnTerminating();
         OnTerminated();
      }

      internal void SendMessage(MessageCode messageCode, params object[] paramsList)
      {
         string message = MessagesRepository.GetMessage(messageCode, paramsList);
         SendMessage(message);
      }

      internal void SendMessage(string message)
      {
         Provider.NotifyObservers(message);
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

      internal virtual void OnError(Exception exception)
      {
         _status = AlgorithmStatus.Error;
         if (Solver.IsMonitoringEnabled)
         {
            Debug.WriteLine(exception.StackTrace);
            _provider.NotifyError(exception);
         }
      }
      #endregion
   }
}