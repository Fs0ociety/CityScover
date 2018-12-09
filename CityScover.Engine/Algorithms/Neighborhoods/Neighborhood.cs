//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/12/2018
//
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal abstract class Neighborhood<TSolution>
   {
      #region Internal properties
      internal AlgorithmType Type { get; set; }
      #endregion

      #region Internal abstract methods
      internal abstract IEnumerable<TSolution> GeneratingLogic(in TSolution solution);
      #endregion
   }
}
