//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 01/10/2018
//

using CityScover.Commons;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

      internal Configuration WorkingConfiguration => Solver.Instance.WorkingConfiguration;
      #endregion

      #region Internal methods
      internal void Evaluate(TOSolution solution)
      {
         Debug.WriteLine($"{nameof(SolverEvaluator)} begin Evaluate() of solution {solution.Id}");
         var objectiveFunc = Solver.Problem.ObjectiveFunc;
         solution.Cost = objectiveFunc.Invoke(solution);

         // Get the violated constraints to invoke the PenaltyFunc delegate.
         var violatedConstraints = (from constraint in solution.ProblemConstraints
                                    where constraint.Value == false
                                    select constraint);

         var penaltyFunc = Solver.Problem.PenaltyFunc;

         StringBuilder builder = default;
         foreach (var constraint in violatedConstraints)
         {
            builder = new StringBuilder();
            var penalty = penaltyFunc.Invoke(solution);
            solution.Cost += penalty;
            builder.Append("violated constraint " + constraint.Key + " - assigned penalty of: " + penalty);
         }

         var debugEvalMsg = $"{nameof(SolverEvaluator)} end Evaluate() of solution {solution.Id} with cost {solution.Cost}";
         if (builder != null)
         {
            debugEvalMsg += '\n' + builder.ToString();
         }
         Debug.WriteLine(debugEvalMsg);
      }
      #endregion
   }
}
