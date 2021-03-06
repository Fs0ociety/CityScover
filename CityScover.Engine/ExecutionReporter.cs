﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 27/11/2018
//

using System;
using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represents the object involved to create the right Algorithm 
   /// and start execution of all stages of the Algorithm.
   /// In addition, it monitors the execution of the Algorithm.
   /// </summary>
   internal sealed class ExecutionReporter : IObserver<ToSolution>
   {
      #region Private fields
      private IDisposable _unsubscriber;
      //private Stopwatch _timer;
      #endregion

      #region Private properties
      private Solver Solver => Solver.Instance;
      #endregion

      #region Internal methods
      /// <summary>
      /// Set its own provider passed as parameter to receive notifications 
      /// and gets the unsubscriber object to cancel subscription.
      /// </summary>
      /// <param name="provider"></param>
      internal void Subscribe(AlgorithmTracker provider) => _unsubscriber = provider.Subscribe(this);

      /// <summary>
      /// Cancel the subscription to provider.
      /// </summary>
      internal void Unsubscribe() => _unsubscriber.Dispose();

      /// <summary>
      /// Invoke the algorithm passed as argument and reports its running time.
      /// </summary>
      /// <param name="algorithm">Algorithm to execute.</param>
      /// <returns></returns>
      internal async Task Run(Algorithm algorithm)
      {
         await Task.Run(algorithm.Start);         
      }
      #endregion

      #region Public methods
      public void OnNextMessage(string message)
      {
         //Console.WriteLine($"\n{nameof(ExecutionReporter)} received message: {message}\n");
         Console.WriteLine($"\n{message}\n");
      }
      #endregion

      #region IObserver implementation
      public void OnNext(ToSolution solution)
      {
         Task taskToAwait = Solver.AlgorithmTasks[solution.Id];

         try
         {
            taskToAwait.Wait();
         }
         catch (AggregateException ae)
         {
            OnError(ae);
         }

         string message = string.Empty;
         string errMessage = string.Empty;
         if (solution.IsValid)
         {
            message = MessagesRepository.GetMessage(MessageCode.EXREPSolutionReceived, solution.Id, solution.Cost);
         }
         else
         {
            message = MessagesRepository.GetMessage(MessageCode.EXREPSolutionReceivedWithPenalty, solution.Id, solution.Cost, Math.Abs(solution.Penalty));
            errMessage = solution.ViolatedConstraintsToString();
         }

         Console.WriteLine(message);
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine(errMessage);
         Console.ForegroundColor = ConsoleColor.Gray;
      }
   
      public void OnError(Exception error)
      {
         Console.WriteLine(MessagesRepository.GetMessage(MessageCode.EXREPExceptionOccurred, error.Message, error.StackTrace));
         throw error;
      }

      public void OnCompleted()
      {
         //string algorithmDescription = Solver.CurrentStage.Flow.CurrentAlgorithm.ToString();
         //TimeSpan elapsedTime = _timer.Elapsed;
         TimeSpan elapsedTime = Solver.CurrentStageExecutionTime;
         string elapsedTimeMsg = MessagesRepository.GetMessage(MessageCode.EXREPTimeFormat, elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
         Console.WriteLine(MessagesRepository.GetMessage(MessageCode.EXREPAlgorithmPerformance, Solver.CurrentAlgorithm.ToString(), elapsedTimeMsg));
      }
      #endregion
   }
}