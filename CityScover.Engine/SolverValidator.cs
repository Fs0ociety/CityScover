//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 12/09/2018
//

using CityScover.Commons;
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
      private BlockingCollection<BaseSolution> _evaluatingQueue;
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
      private BaseSolution Validate(BaseSolution solution)
      {
         var problemConstraints = Solver.Problem.Constraints;
         var relaxedConstraintsId = WorkingConfiguration.RelaxedConstraintsId;

         // Get the constraints delegates to be invoked.
         var checkingConstraints = from problemConstraint in problemConstraints
                                   where !(from relaxedConstraintId in relaxedConstraintsId
                                           where relaxedConstraintsId.Equals(problemConstraint.Key)
                                           select relaxedConstraintId).Any() && problemConstraint.Value != null
                                   select problemConstraint;

         foreach (var constraint in checkingConstraints)
         {
            bool isValid = constraint.Value.Invoke(solution);
            //solution.ProblemConstraints.Add(constraint.Key, isValid);
         }

         return solution;
      }
   
      private async Task TakeSolutionsToValidate()
      {
         var validatingQueue = Solver.ValidatingQueue;
         foreach (var validatingSolution in validatingQueue.GetConsumingEnumerable())
         {
            Task validatingTask = Task.Run(delegate
            {
               BaseSolution validatedSolution = Validate(validatingSolution);
               _evaluatingQueue.Add(validatedSolution);
            });

            _processingTasks.Add(validatingTask);
         }

         await Task.WhenAll(_processingTasks.ToArray());
         _evaluatingQueue.CompleteAdding();
      }
      #endregion

      #region Internal methods
      internal async Task Run()
      {
         await TakeSolutionsToValidate().ConfigureAwait(continueOnCapturedContext: false);
      }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         _evaluatingQueue = new BlockingCollection<BaseSolution>();
         _processingTasks = new Collection<Task>();
      }
      #endregion
   }
}
