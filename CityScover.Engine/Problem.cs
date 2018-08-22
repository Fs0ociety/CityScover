//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/08/2018
//

using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   /// <summary>
   /// This struct defines the template of a operating research problem.
   /// </summary>
   internal struct Problem
   {
      private readonly IEnumerable<IConstraint> _constraints;
      private readonly IEnumerable<IObjectiveFunction> _objectiveFunctions;

      #region Constructors
      internal Problem(IEnumerable<IConstraint> constraints, IEnumerable<IObjectiveFunction> objectiveFunctions)
      {
         _constraints = constraints ?? throw new ArgumentNullException(nameof(Problem));
         _objectiveFunctions = objectiveFunctions ?? throw new ArgumentNullException(nameof(Problem));
      } 
      #endregion

      #region Internal properties
      internal IEnumerable<IConstraint> Constraints => _constraints;
      internal IEnumerable<IObjectiveFunction> ObjectiveFunctions => _objectiveFunctions; 
      #endregion
   }
}
