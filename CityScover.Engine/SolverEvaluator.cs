//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/11/2018
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

      #region Private properties
      private Solver Solver => Solver.Instance;
      #endregion

      #region Internal methods
      internal void Evaluate(ToSolution solution)
      {
         var objectiveFunc = Solver.Problem.ObjectiveFunc;
         solution.Cost = objectiveFunc.Invoke(solution);

         var penaltyFunc = Solver.Problem.PenaltyFunc;

         // Get the violated constraints to invoke the PenaltyFunc delegate.
         var violatedConstraints = solution.ProblemConstraints
            .Where(constraint => constraint.Value == false)
            .ToList();

         violatedConstraints.ForEach(
            delegate
            {
               int penalty = penaltyFunc.Invoke(solution);
               solution.Cost += penalty;
               solution.Penalty = penalty < 0 ? -penalty : penalty;
            });
      }
      #endregion
   }
}
