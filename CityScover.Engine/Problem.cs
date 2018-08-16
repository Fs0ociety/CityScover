//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal class Problem
   {
      private readonly IEnumerable<Constraint> _constraints;
      private readonly IEnumerable<ObjectiveFunction> _objectiveFunctions;

      #region Constructors
      internal Problem()
      {
      }

      internal Problem(IEnumerable<Constraint> constraints, IEnumerable<ObjectiveFunction> objectiveFunctions)
      {
         _constraints = constraints ?? throw new ArgumentNullException(nameof(Problem));
         _objectiveFunctions = objectiveFunctions ?? throw new ArgumentNullException(nameof(Problem));
      } 
      #endregion

      #region Internal properties
      internal IEnumerable<Constraint> Constraints => _constraints;
      internal IEnumerable<ObjectiveFunction> ObjectiveFunctions => _objectiveFunctions; 
      #endregion
   }
}
