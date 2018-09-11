//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 11/09/2018
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   internal class TOProblem : Problem
   {
      private Func<Solution, int> _objectiveFunc;

      #region Constructors
      internal TOProblem()
         : base()
      {
         ObjectiveFunc = CalculateCost;

         Constraints.Add(
            new KeyValuePair<byte, Func<Solution, bool>>(1, IsTMaxConstraintSatisfied));
         Constraints.Add(
            new KeyValuePair<byte, Func<Solution, bool>>(2, IsTimeWindowsConstraintSatisfied));         
      }
      #endregion

      #region Overrides
      internal override Func<Solution, int> ObjectiveFunc
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
      private int CalculateCost(Solution solution)
      {
         var solutionNodes = solution.SolutionGraph.Nodes;
         return solutionNodes.Sum(node => node.Entity.Score.Value);
      }
      #endregion

      #region Constraints delegates
      private bool IsTimeWindowsConstraintSatisfied(Solution solution)
      {
         throw new NotImplementedException();
      }

      private bool IsTMaxConstraintSatisfied(Solution solution)
      {
         throw new NotImplementedException();
      }
      #endregion
   }
}