//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 01/10/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// This abstract class represents a template for a generic problem.
   /// </summary>
   internal abstract class Problem
   {
      #region Constructors
      internal Problem()
      {
         Constraints = new Collection<KeyValuePair<byte, Func<TOSolution, bool>>>();
      }
      #endregion

      #region Abstract members
      /// <summary>
      /// Abstract automatic properties doesn't create a private backing field.
      /// </summary>
      internal abstract Func<TOSolution, int> ObjectiveFunc { get; set; }

      internal abstract Func<TOSolution, int> PenaltyFunc { get; set; }
      #endregion

      #region Internal properties
      internal ICollection<KeyValuePair<byte, Func<TOSolution, bool>>> Constraints { get; }
      internal bool IsMinimizing { get; set; } = true;
      #endregion
   }
}
