//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 11/09/2018
//

using CityScover.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal sealed class SolverValidator : Singleton<SolverValidator>
   {
      private BlockingCollection<Solution> _evaluatingQueue;
      private ICollection<Task> _processingTasks;

      #region Constructors
      private SolverValidator()
      {
      }
      #endregion

      #region Internal properties
      internal Solver Solver => Solver.Instance;

      internal Configuration WorkingConfiguration
      {
         get => Solver.Instance.WorkingConfiguration;
      }
      #endregion

      #region Private methods
      private async ValueTask<Solution> Validate(Solution solution)
      {
         var problemConstraints = Solver.Problem.Constraints;
         var relaxedConstraintsId = WorkingConfiguration.RelaxedConstraintsId;

         // Get the constraints delegate to be invoked.
         var checkingConstraints = problemConstraints.Zip(relaxedConstraintsId,
            (problemConstraint, relaxedConstraintId) => (relaxedConstraintId != problemConstraint.Key) 
            ? problemConstraint : new KeyValuePair<byte, Func<Solution, bool>>())
            .Where(x => x.Value != null)
            .Select(x => x);

         foreach (var constraint in checkingConstraints)
         {
            bool isValid = constraint.Value.Invoke(solution);
            solution.ProblemConstraints.Add(constraint.Key, isValid);
         }

         //solution.IsValid = GetRandomBoolean();
         bool GetRandomBoolean()
         {
            return new Random().Next(100) % 2 == 0;
         }
         return solution;
      }
      #endregion

      #region Internal methods
      internal async Task Run()
      {
         await TakeSolutionsToValidate().ConfigureAwait(false);
      }

      private async Task TakeSolutionsToValidate()
      {
         var validatingQueue = Solver.ValidatingQueue;
         foreach (var validatingSolution in validatingQueue.GetConsumingEnumerable())
         {
            // Attacca una continuazione di un Task sulla CPU.
            Task validationTask = Task.Run(async delegate
            {
               Solution validatedSolution = await Validate(validatingSolution);
               _evaluatingQueue.Add(validatedSolution);
            });

            _processingTasks.Add(validationTask);
         }

         await Task.WhenAll(_processingTasks.ToArray());
         _evaluatingQueue.CompleteAdding();
      }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         _evaluatingQueue = new BlockingCollection<Solution>();
         _processingTasks = new Collection<Task>();
      }
      #endregion
   }
}
