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
   /// <summary>
   /// TODO
   /// </summary>
   internal abstract class Problem
   {
      internal struct Constraint
      {
         /// <summary>
         /// Abstraction of problem's constraint.
         /// </summary>
         /// <param name="id"></param>
         /// <param name="logic"></param>
         internal Constraint(byte id, Func<Solution, bool> logic)
         {
            Id = id;
            Logic = logic;
         }

         internal byte Id { get; set; }
         internal Func<Solution, bool> Logic { get; set; }
      }

      #region Constructors
      internal Problem()
      { }
      #endregion

      #region Internal properties
      internal Func<Solution, Evaluation> ObjectiveFunc { get; set; } = default;
      internal ICollection<Constraint> Constraints { get; } = new Collection<Constraint>();
      #endregion
   }
}
