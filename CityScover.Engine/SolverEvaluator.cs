//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 04/10/2018
//

using CityScover.Commons;
using System.Linq;

namespace CityScover.Engine
{
   /// <summary>
   /// This class has the responsability to evaluate a Solution received from the Solver instance.
   /// </summary>
   internal sealed class SolverEvaluator : Singleton<SolverEvaluator>
   {
      #region Constructors
      private SolverEvaluator()
      {
      }
      #endregion

      #region Internal properties
      internal Solver Solver => Solver.Instance;

      internal Configuration WorkingConfiguration => Solver.Instance.WorkingConfiguration;
      #endregion

      #region Internal methods
      internal void Evaluate(TOSolution solution)
      {
         var objectiveFunc = Solver.Problem.ObjectiveFunc;
         solution.Cost = objectiveFunc.Invoke(solution);

         // Get the violated constraints to invoke the PenaltyFunc delegate.
         var violatedConstraints = (from constraint in solution.ProblemConstraints
                                    where constraint.Value == false
                                    select constraint);

         var penaltyFunc = Solver.Problem.PenaltyFunc;

         foreach (var constraint in violatedConstraints)
         {
            var penalty = penaltyFunc.Invoke(solution);
            solution.Cost += penalty;
            solution.Penalty = penalty;
         }
      }
      #endregion
   }
}
