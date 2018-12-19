//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 19/12/2018
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
      IEnumerable<KeyValuePair<string, Func<ToSolution, bool>>> _checkingConstraints;

      #region Constructors
      private SolverValidator()
      {
      }
      #endregion

      #region Private properties
      private Solver Solver => Solver.Instance;
      #endregion

      #region Internal methods
      internal void Validate(ToSolution solution)
      {
         var problemConstraints = Solver.Problem.Constraints;

         // Get the constraints delegates to be invoked.
         var checkingConstraints = from problemConstraint in problemConstraints
                                   where !(from relaxedConstraint in Solver.ConstraintsToRelax
                                           where relaxedConstraint.Equals(problemConstraint.Key)
                                           select relaxedConstraint).Any() && problemConstraint.Value != null
                                   select problemConstraint;

         foreach (var constraint in checkingConstraints)
         {
            bool isValid = constraint.Value.Invoke(solution);
            solution.ProblemConstraints.Add(constraint.Key, isValid);
         }
      }
      #endregion

      internal void InitializeProblemConstraints()
      {
         _checkingConstraints = from problemConstraint in Solver.Problem.Constraints
                                where !(from relaxedConstraint in Solver.ConstraintsToRelax
                                        where relaxedConstraint.Equals(problemConstraint.Key)
                                        select relaxedConstraint).Any() && problemConstraint.Value != null
                                select problemConstraint;
      }
   }
}