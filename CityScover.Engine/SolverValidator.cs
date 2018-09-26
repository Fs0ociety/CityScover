//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 26/09/2018
//

using CityScover.Commons;
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

      internal Configuration WorkingConfiguration
      {
         get => Solver.Instance.WorkingConfiguration;
      }
      #endregion

      #region Internal methods
      internal TOSolution Validate(TOSolution solution)
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
            solution.ProblemConstraints.Add(constraint.Key, isValid);
         }

         return solution;
      }
      #endregion
   }
}
