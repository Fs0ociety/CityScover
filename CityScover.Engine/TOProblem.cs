//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 10/09/2018
//

using System;

namespace CityScover.Engine
{
   internal class TOProblem : Problem
   {
      #region Constructors
      internal TOProblem()
         : base()
      {
         ObjectiveFunc = CalculateCost;

         Constraints.Add(new Constraint()
         {
            Id = 1,
            Logic = IsTMaxConstraintSatisfied
         });
         Constraints.Add(new Constraint()
         {
            Id = 2,
            Logic = IsTimeWindowsConstraintSatisfied
         });
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
      private Evaluation CalculateCost(Solution solution)
      {
         throw new NotImplementedException();
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