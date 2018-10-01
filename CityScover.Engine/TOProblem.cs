//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 01/10/2018
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   internal class TOProblem : Problem
   {
      private const int PenaltyAmount = 100;

      private readonly DateTime _tMax;
      private Func<TOSolution, int> _objectiveFunc;
      private Func<TOSolution, int> _penaltyFunc;

      #region Constructors
      internal TOProblem()
         : base()
      {
         var solverConfig = Solver.Instance.WorkingConfiguration;
         _tMax = solverConfig.ArrivalTime.Add(solverConfig.TourDuration);
         ObjectiveFunc = CalculateCost;
         PenaltyFunc = CalculatePenalty;
         IsMinimizing = false;

         Constraints.Add(
            new KeyValuePair<byte, Func<TOSolution, bool>>(1, IsTMaxConstraintSatisfied));
         //Constraints.Add(
         //   new KeyValuePair<byte, Func<TOSolution, bool>>(2, IsTimeWindowsConstraintSatisfied));
      }
      #endregion

      #region Overrides
      internal override Func<TOSolution, int> ObjectiveFunc
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

      internal override Func<TOSolution, int> PenaltyFunc
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
      private int CalculateCost(TOSolution solution)
      {
         var solutionNodes = solution.SolutionGraph.Nodes;
         return (from node in solutionNodes
                 select node.Entity.Score.Value).Sum();
      }
      #endregion

      #region Penalty Function delegates
      private int CalculatePenalty(TOSolution solution)
      {
         return PenaltyAmount;
      }
      #endregion

      #region Constraints delegates
      private bool IsTimeWindowsConstraintSatisfied(TOSolution solution)
      {
         throw new NotImplementedException();
      }

      private bool IsTMaxConstraintSatisfied(TOSolution solution)
      {
         return solution.TimeSpent <= _tMax;
      }
      #endregion
   }
}