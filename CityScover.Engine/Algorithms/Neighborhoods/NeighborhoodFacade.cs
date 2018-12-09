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

using System;
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class NeighborhoodFacade<TSolution>
   {
      private readonly Neighborhood<TSolution> _neighborhood;

      #region Constructors
      internal NeighborhoodFacade(Neighborhood<TSolution> neighborhood)
      {
         _neighborhood = neighborhood ?? throw new ArgumentNullException(nameof(neighborhood));
      }
      #endregion

      internal IEnumerable<TSolution> GenerateNeighborhood(in TSolution solution)
      {
         return _neighborhood.GeneratingLogic(solution);
      }
   }
}
