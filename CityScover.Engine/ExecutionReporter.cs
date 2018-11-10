//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 08/11/2018
//

using CityScover.Engine.Algorithms;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represents the object involved to create the right Algorithm 
   /// and start execution of all stages of the Algorithm.
   /// In addition, it monitors the execution of the Algorithm.
   /// </summary>
   internal class ExecutionReporter : IObserver<TOSolution>
   {
      #region Private fields
      private IDisposable _unsubscriber;
      private Stopwatch _timer;
      #endregion

      #region Constructors
      internal ExecutionReporter()
      {
      }
      #endregion

      #region Internal properties
      internal Solver Solver => Solver.Instance;

      public Stopwatch RunningTime { get; set; }
      #endregion

      #region Internal methods
      /// <summary>
      /// Set its own provider passed as parameter to receive notifications 
      /// and gets the unsubscriber object to cancel subscription.
      /// </summary>
      /// <param name="provider"></param>
      internal virtual void Subscribe(AlgorithmTracker provider) => _unsubscriber = provider.Subscribe(this);

      /// <summary>
      /// Cancel the subscription to provider.
      /// </summary>
      internal virtual void Unsubscribe() => _unsubscriber.Dispose();

      /// <summary>
      /// Invoke the algorithm passed as argument and reports its running time.
      /// </summary>
      /// <param name="algorithm">Algorithm to execute.</param>
      /// <returns></returns>
      internal async Task Run(Algorithm algorithm)
      {
         _timer = Stopwatch.StartNew();
         await Task.Run(() => algorithm.Start());
         RunningTime = _timer;
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
      public void OnNext(TOSolution solution)
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
         if (solution.IsValid)
         {
            message = MessagesRepository.GetMessage(MessageCode.EXREPSolutionReceived, solution.Id, solution.Cost);
         }
         else
         {
            message = MessagesRepository.GetMessage(MessageCode.EXREPSolutionReceivedWithPenalty, solution.Id, solution.Cost, solution.Penalty);
            message += "\n" + solution.ViolatedConstraintsToString();
         }

         Console.WriteLine(message);
         //Console.WriteLine($"{nameof(ExecutionReporter)} " +
         //   $"- Solution received: {solution.Id}, COST: {solution.Cost} PENALTY: {solution.Penalty}");
      }
   
      public void OnError(Exception error)
      {
         _timer.Stop();
         Console.WriteLine(MessagesRepository.GetMessage(MessageCode.EXREPExceptionOccurred, error.Message));
         //Console.WriteLine($"{nameof(ExecutionReporter)}: Exception occurred: {error.Message}\n");
         throw error;
      }

      public void OnCompleted()
      {
         _timer.Stop();
         
         string algorithmDescription = Solver.CurrentStage.Flow.CurrentAlgorithm.ToString();
         TimeSpan elapsedTime = _timer.Elapsed;
         string elapsedTimeMsg = MessagesRepository.GetMessage(MessageCode.EXREPTimeFormat, elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
         Console.WriteLine(MessagesRepository.GetMessage(MessageCode.EXREPAlgorithmPerformance, algorithmDescription, elapsedTimeMsg));
         //Console.WriteLine($"The algorithm: {algorithmDescription} performed in " +
            //$"{TimeSpan.FromMilliseconds(_timer.ElapsedMilliseconds)}.\n");

         AlgorithmFamily resultFamily = Result.GetAlgorithmFamily(Solver.CurrentStage.Flow.CurrentAlgorithm);
         Result algorithmResult = Solver.Results.Where(result => result.ResultFamily == resultFamily).FirstOrDefault();
         if (algorithmResult != null)
         {
            algorithmResult.RunningTime = _timer;
         }
      }
      #endregion
   }
}