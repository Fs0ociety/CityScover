//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 11/09/2018
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
         Constraints = new Collection<KeyValuePair<byte, Func<BaseSolution, bool>>>();
      }
      #endregion

      #region Abstract members
      /// <summary>
      /// Abstract automatic properties doesn't create a private backing field.
      /// </summary>
      internal abstract Func<BaseSolution, int> ObjectiveFunc { get; set; }
      #endregion

      #region Internal properties
      internal ICollection<KeyValuePair<byte, Func<BaseSolution, bool>>> Constraints { get; }
      #endregion
   }
}
