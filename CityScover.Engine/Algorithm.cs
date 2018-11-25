//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 25/11/2018
//

using CityScover.Commons;
using CityScover.Engine.Algorithms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// This abstract class represents the interface for a generic Algorithm type.
   /// </summary>
   internal abstract class Algorithm
   {
      #region Private members
      private DateTime _startingTime;
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
         _acceptImprovementsOnly = acceptImprovementsOnly;
         _provider = provider;
         _currentStep = 1;
      }
      #endregion

      #region Private Protected properties
      private protected Solver Solver => Solver.Instance;
      
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

      internal bool ForceStop
      {
         get;
         private protected set;
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

      internal AlgorithmType Type { get; private protected set; }

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
      #endregion

      #region Virtual methods
      internal virtual void OnInitializing()
      {
         _status = AlgorithmStatus.Initializing;
         double scoreWeight = Utils.ObjectiveFunctionWeightDefault;
         if (Parameters.ContainsKey(ParameterCodes.ObjectiveFunctionScoreWeight))
         {
            double scoreParam = Parameters[ParameterCodes.ObjectiveFunctionScoreWeight];
            if (scoreWeight >= 0 && scoreWeight <= 1)
            {
               scoreWeight = scoreParam;
            }
         }
         Solver.CurrentObjectiveFunctionWeight = scoreWeight;

         if (Parameters.ContainsKey(ParameterCodes.RelaxedConstraints))
         {
            Solver.ConstraintsToRelax.Clear();
            foreach (string relaxedConstraint in Parameters[ParameterCodes.RelaxedConstraints])
            {
               Solver.ConstraintsToRelax.Add(relaxedConstraint);
            }
         }

         _startingTime = DateTime.Now;
      }

      internal virtual void OnTerminating()
      {
         _status = AlgorithmStatus.Terminating;
      }

      internal virtual void OnTerminated()
      {
         _status = AlgorithmStatus.Terminated;         
         Solver.CurrentStageExecutionTime = Solver.CurrentStageExecutionTime.Add(DateTime.Now - _startingTime);
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

      internal virtual bool StopConditions()
      {
         return ForceStop || _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}