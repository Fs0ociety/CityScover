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
   internal class TOProblem : IProblem
   {
      private ICollection<Func<Solution, bool>> _constraints;
      private Func<Solution, Evaluation> _objectiveFunc;

      #region Constructors
      internal TOProblem()
      {
         _constraints = new Collection<Func<Solution, bool>>();
         _constraints.Add(IsTMaxConstraintSatisfied);
         _constraints.Add(IsTimeWindowsConstraintSatisfied);
         //TODO: aggiunta vincoli, pensare se fare struct (che estendono Constraint, e si trovano dentro
         //      una cartella Constraints) o delegati all'interno di questa classe.
         ObjectiveFunc = CalculateCost;
      }
      #endregion

      #region Delegates

      #region Objective Function
      private Evaluation CalculateCost(Solution solution)
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Constraints
      private bool IsTimeWindowsConstraintSatisfied(Solution arg)
      {
         throw new NotImplementedException();
      }

      private bool IsTMaxConstraintSatisfied(Solution arg)
      {
         throw new NotImplementedException();
      }
      #endregion

      #endregion

      #region Interface implementations
      public ICollection<Func<Solution, bool>> Constraints
      {
         get => _constraints;
         set
         {
            if (_constraints != value)
            {
               _constraints = value;
            }
         }
      }

      public Func<Solution, Evaluation> ObjectiveFunc
      {
         get => _objectiveFunc;
         set
         {
            if (_objectiveFunc != value)
            {
               _objectiveFunc = value;
            }
         }
      }

      #endregion
   }
}
