//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 18/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Diagnostics;
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

      #region Private methods
      private void DisplaySolutionGraph(TOSolution solution)
      {
         CityMapGraph solutionGraph = solution.SolutionGraph;
         string result = String.Empty;

         int startPOIId = Solver.WorkingConfiguration.StartingPointId;
         solutionGraph.BreadthFirstSearch(startPOIId,
            (node, isVisited) => node.IsVisited = isVisited,
            (node) => { return node.IsVisited; },
            node => result += $"({node.Entity.Id} -- {node.Entity.Name})",
            edge => {
               if (edge.Entity.PointTo.Id != startPOIId)
               {
                  result += $" --> ";
               }
            });
         Console.WriteLine(result);
      }
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

         DisplaySolutionGraph(Solver.BestSolution);
         string algorithmDescription = Solver.CurrentStage.Flow.CurrentAlgorithm.ToString();
         Console.WriteLine($"The algorithm: {algorithmDescription} performed in " +
            $"{TimeSpan.FromMilliseconds(_timer.ElapsedMilliseconds)}.\n");

         //AlgorithmFamily resultType = Result.GetAlgorithmFamilyByType(Solver.CurrentStage.Flow.CurrentAlgorithm);
         //Result algorithmResult = Solver.Results[resultType];
         //algorithmResult.RunningTime = _timer;
      }
      #endregion
   }
}