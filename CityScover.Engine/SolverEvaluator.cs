//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 15/09/2018
//

using CityScover.Commons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal sealed class SolverEvaluator : Singleton<SolverEvaluator>
   {
      private ICollection<Task> _processingTasks;

      #region Constructors
      private SolverEvaluator()
      {
      }
      #endregion

      #region Internal properties
      internal Solver Solver => Solver.Instance;

      internal SolverValidator SolverValidator => SolverValidator.Instance;

      internal Configuration WorkingConfiguration
      {
         get => Solver.Instance.WorkingConfiguration;
      }
      #endregion

      #region Private methods
      private TOSolution Evaluate(TOSolution solution)
      {
         var objectiveFunc = Solver.Problem.ObjectiveFunc;
         solution.Cost = objectiveFunc.Invoke(solution); ;

         // Get the violated constraints to invoke the PenaltyFunc delegate.
         var violatedConstraints = (from constraint in solution.ProblemConstraints.Values
                                    where constraint == false
                                    select constraint);

         var penaltyFunc = Solver.Problem.PenaltyFunc;
         foreach (var constraint in violatedConstraints)
         {
            solution.Cost = penaltyFunc.Invoke(solution);
         }
         
         return solution;
      }

      private async Task TakeSolutionsToEvaluate()
      {
         var evaluatingQueue = SolverValidator.EvaluatingQueue;
         var evaluatedQueue = Solver.EvaluatedQueue;
         foreach (var evaluatingSolution in evaluatingQueue.GetConsumingEnumerable())
         {
            Task evaluatingTask = Task.Run(delegate
            {
               TOSolution evaluatedSolution = Evaluate(evaluatingSolution);
               evaluatedQueue.Add(evaluatedSolution);
            });

            _processingTasks.Add(evaluatingTask);
         }

         await Task.WhenAll(_processingTasks);
         evaluatedQueue.CompleteAdding();
      }
      #endregion

      #region Internal methods
      internal async Task Run()
      {
         await TakeSolutionsToEvaluate().ConfigureAwait(continueOnCapturedContext: false);
      }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         _processingTasks = new Collection<Task>();
      } 
      #endregion
   }
}
