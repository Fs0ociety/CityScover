//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 29/09/2018
//

using CityScover.Commons;
using System.Linq;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
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

      internal SolverValidator SolverValidator => SolverValidator.Instance;

      internal Configuration WorkingConfiguration
      {
         get => Solver.Instance.WorkingConfiguration;
      }
      #endregion

      #region Internal methods
      internal void Evaluate(TOSolution solution)
      {
         var objectiveFunc = Solver.Problem.ObjectiveFunc;
         solution.Cost = objectiveFunc.Invoke(solution);

         // Get the violated constraints to invoke the PenaltyFunc delegate.
         var violatedConstraints = (from constraint in solution.ProblemConstraints.Values
                                    where constraint == false
                                    select constraint);

         var penaltyFunc = Solver.Problem.PenaltyFunc;
         foreach (var constraint in violatedConstraints)
         {
            solution.Cost = penaltyFunc.Invoke(solution);
         }
      }
      #endregion
   }
}
