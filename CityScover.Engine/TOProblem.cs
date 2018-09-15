//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/09/2018
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   internal class TOProblem : Problem
   {
      private Func<TOSolution, double> _objectiveFunc;
      private Func<TOSolution, double> _penaltyFunc;

      #region Constructors
      internal TOProblem()
         : base()
      {
         ObjectiveFunc = CalculateCost;
         IsMinimizing = false;

         Constraints.Add(
            new KeyValuePair<byte, Func<TOSolution, bool>>(1, IsTMaxConstraintSatisfied));
         Constraints.Add(
            new KeyValuePair<byte, Func<TOSolution, bool>>(2, IsTimeWindowsConstraintSatisfied));
      }
      #endregion

      #region Overrides
      internal override Func<TOSolution, double> ObjectiveFunc
      {
         get => _objectiveFunc;
         set
         {
            if (value != _objectiveFunc)
            {
               _objectiveFunc = value;
            }
         }
      }

      internal override Func<TOSolution, double> PenaltyFunc
      {
         get => _penaltyFunc;
         set
         {
            if (value != _penaltyFunc)
            {
               _penaltyFunc = value;
            }
         }
      }
      #endregion

      #region Objective Function delegates
      /// <summary>
      /// Calculates the solution cost passed as parameter.
      /// </summary>
      /// <param name="solution">
      /// Solution to evaluate.
      /// </param>
      /// <returns>
      /// An Evaluation Object.
      /// </returns>
      private double CalculateCost(TOSolution solution)
      {
         var solutionNodes = solution.SolutionGraph.Nodes;
         return (from node in solutionNodes
                 select node.Entity.Score.Value).Sum();
      }
      #endregion

      #region Penalty Function delegates
      private double CalculatePenalty(TOSolution solution)
      {
         var solutionCost = solution.Cost;
         return solutionCost + new Random().Next(10);
      }
      #endregion

      #region Constraints delegates
      private bool IsTimeWindowsConstraintSatisfied(TOSolution solution)
      {
         throw new NotImplementedException();
      }

      private bool IsTMaxConstraintSatisfied(TOSolution solution)
      {
         throw new NotImplementedException();
      }
      #endregion
   }
}