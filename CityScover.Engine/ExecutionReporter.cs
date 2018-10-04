//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 04/10/2018
//

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
      private IDisposable _unsubscriber;
      private Stopwatch _timer;

      #region Constructors
      internal ExecutionReporter()
      {
         // TODO
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
         if (algorithm == null)
         {
            throw new ArgumentNullException(nameof(algorithm));
         }

         _timer = Stopwatch.StartNew();
         await Task.Run(() => algorithm.Start());
         RunningTime = _timer;
      }
      #endregion

      #region IObserver implementation
      public void OnNext(TOSolution solution)
      {
         Task taskToAwait = Solver.AlgorithmTasks[solution.Id];
         Task.WaitAll(taskToAwait);
         Console.WriteLine($"{nameof(ExecutionReporter)} " +
            $"- Solution received: {solution.Id}, COST: {solution.Cost} PENALTY: {solution.Penalty}");
      }
   
      public void OnError(Exception error)
      {
         _timer.Stop();
         Console.WriteLine($"{nameof(ExecutionReporter)}: Exception occurred!\n");
         throw error;
      }

      public void OnCompleted()
      {
         _timer.Stop();
         string algorithmDescription = Solver.CurrentStage.Flow.CurrentAlgorithm.ToString();
         Console.WriteLine($"The algorithm: {algorithmDescription} performed in " +
            $"{TimeSpan.FromMilliseconds(_timer.ElapsedMilliseconds)}.");

         ResultType resultType = Result.GetResultTypeFromAlgorithmType(Solver.CurrentStage.Flow.CurrentAlgorithm);
         Result algorithmResult = Solver.Results[resultType];
         algorithmResult.RunningTime = _timer;
      }
      #endregion
   }
}
