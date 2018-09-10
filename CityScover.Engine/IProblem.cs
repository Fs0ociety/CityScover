//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 10/09/2018
//

using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   internal interface IProblem
   {
      #region Internal properties
      ICollection<Func<Solution, bool>> Constraints { get; set; }

      Func<Solution, Evaluation> ObjectiveFunc { get; set; }
      #endregion
   }
}
