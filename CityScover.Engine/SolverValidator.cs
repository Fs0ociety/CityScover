//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 20/12/2018
//

using CityScover.Commons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   /// <summary>
   /// This class has the responsability to validate a Solution received from the Solver instance.
   /// </summary>
   internal sealed class SolverValidator : Singleton<SolverValidator>
   {
      IEnumerable<KeyValuePair<string, Func<ToSolution, bool>>> _constraintsToValidate;

      #region Constructors
      private SolverValidator()
      {
      }
      #endregion

      #region Private properties
      private Solver Solver => Solver.Instance;
      #endregion

      #region Internal methods
      internal void InitializeProblemConstraints()
      {
         // Get the constraints delegates to be invoked.
         _constraintsToValidate =
            from problemConstraint in Solver.Problem.Constraints
            where !(from relaxedConstraint in Solver.ConstraintsToRelax
                    where relaxedConstraint.Equals(problemConstraint.Key)
                    select relaxedConstraint).Any() && problemConstraint.Value != null
            select problemConstraint;
      }
   
      internal void Validate(ToSolution solution)
      {
         foreach (var constraint in _constraintsToValidate)
         {
            bool isValid = constraint.Value.Invoke(solution);
            solution.ProblemConstraints.Add(constraint.Key, isValid);
         }
      }
      #endregion
   }
}