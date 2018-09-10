//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 10/09/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   internal interface IProblem
   {
      #region Internal properties
      ICollection<Constraint> Constraints { get; set; }

      Func<Solution, Evaluation> ObjectiveFunc { get; set; }
      #endregion
   }

   internal struct Constraint
   {
      internal byte Id { get; set; }
      internal Func<Solution, bool> Logic { get; set; }
   }

   internal class MyProblemExample : IProblem
   {
      private ICollection<Constraint> _constraints;
      private Func<Solution, Evaluation> _objectiveFunc;

      internal MyProblemExample() : base()
      {
         Constraints = new Collection<Constraint>();
         Constraints.Add(new Constraint() {
            Id = 1,
            Logic = IsTMaxConstraintSatisfied
         });
         Constraints.Add(new Constraint() {
            Id = 2,
            Logic = IsTimeWindowsConstraintSatisfied
         });

         ObjectiveFunc = CalculateCost;
      }

      public ICollection<Constraint> Constraints {
         get => _constraints;
         set => _constraints = value;
      }

      public Func<Solution, Evaluation> ObjectiveFunc {
         get => _objectiveFunc;
         set => _objectiveFunc = value;
      }

      #region Delegates
      #region Objective Function
      private Evaluation CalculateCost(Solution solution) { throw new NotImplementedException(); }
      #endregion

      #region Constraints
      private bool IsTimeWindowsConstraintSatisfied(Solution arg) { throw new NotImplementedException(); }
      private bool IsTMaxConstraintSatisfied(Solution arg) { throw new NotImplementedException(); }
      // Altri delegati per il check dei vincoli
      #endregion
      #endregion
   }
}
