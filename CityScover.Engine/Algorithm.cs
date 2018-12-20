//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 19/12/2018
//

using CityScover.Commons;
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
      private DateTime _startingTime;
      #endregion

      #region Constructors
      internal Algorithm(AlgorithmTracker provider, bool acceptImprovementsOnly = true)
      {
         AcceptImprovementsOnly = acceptImprovementsOnly;
         Provider = provider;
         CurrentStep = 1;
      }
      #endregion

      #region Private Protected properties
      private protected Solver Solver => Solver.Instance;

      /// <summary>
      /// Returns the current Status of the Algorithm.
      /// See the AlgorithmStatus types for details.
      /// </summary>
      private protected AlgorithmStatus Status { get; private set; }
   
      private protected ushort CurrentStep { get; set; }

      private protected bool ForceStop { get; set; }
      #endregion
      
      #region Internal properties
      internal bool AcceptImprovementsOnly { get; set; }

      internal AlgorithmTracker Provider { get; set; }

      internal AlgorithmType Type { get; private protected set; }

      internal IDictionary<ParameterCodes, dynamic> Parameters { get; set; }
      #endregion

      #region Internal methods
      internal async Task Start()
      {
         OnInitializing();

         Status = AlgorithmStatus.Running;

         while (!StopConditions())
         {
            try
            {
               await Task.Run(PerformStep);
               CurrentStep++;
            }
            catch (Exception ex)
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

      protected abstract Task PerformStep();
      #endregion

      #region Virtual methods
      internal virtual void OnInitializing()
      {
         Status = AlgorithmStatus.Initializing;
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
            foreach (var relaxedConstraint in Parameters[ParameterCodes.RelaxedConstraints])
            {
               Solver.ConstraintsToRelax.Add(relaxedConstraint);
            }
         }

         Solver.InitConstraintsToValidate();
         _startingTime = DateTime.Now;
      }

      internal virtual void OnTerminating()
      {
         Status = AlgorithmStatus.Terminating;
      }

      internal virtual void OnTerminated()
      {
         Status = AlgorithmStatus.Terminated;
         Solver.CurrentStageExecutionTime = Solver.CurrentStageExecutionTime.Add(DateTime.Now - _startingTime);
         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyCompletion();
         }
      }

      internal virtual void OnError(Exception exception)
      {
         Status = AlgorithmStatus.Error;
         if (Solver.IsMonitoringEnabled)
         {
            Debug.WriteLine(exception.StackTrace);
            Provider.NotifyError(exception);
         }
      }

      internal virtual bool StopConditions()
      {
         return ForceStop || Status == AlgorithmStatus.Error;
      }
      #endregion
   }
}