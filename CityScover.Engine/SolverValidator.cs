//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 02/10/2018
//

using CityScover.Commons;
using System.Diagnostics;
using System.Linq;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal sealed class SolverValidator : Singleton<SolverValidator>
   {
      #region Constructors
      private SolverValidator()
      {
      }
      #endregion

      #region Internal properties
      internal Solver Solver => Solver.Instance;

      internal Configuration WorkingConfiguration => Solver.Instance.WorkingConfiguration;
      #endregion

      #region Internal methods
      internal void Validate(TOSolution solution)
      {
         Debug.WriteLine($"{nameof(SolverValidator)} begin Validate() of solution {solution.Id}");
         var problemConstraints = Solver.Problem.Constraints;

         // Get the constraints delegates to be invoked.
         var checkingConstraints = from problemConstraint in problemConstraints
                                   where !(from relaxedConstraintId in Solver.ConstraintsToRelax
                                           where relaxedConstraintId.Equals(problemConstraint.Key)
                                           select relaxedConstraintId).Any() && problemConstraint.Value != null
                                   select problemConstraint;

         foreach (var constraint in checkingConstraints)
         {
            bool isValid = constraint.Value.Invoke(solution);
            solution.ProblemConstraints.Add(constraint.Key, isValid);
         }
         Debug.WriteLine($"{nameof(SolverValidator)} end Validate() of solution {solution.Id}");
      }
      #endregion
   }
}
