//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 06/10/2018
//

using CityScover.Engine.Algorithms.LocalSearches;
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal class TabuSearchTwoOptNeighborhood : TabuSearchNeighborhood
   {
      private TwoOptNeighborhood _neighborhood;

      internal TabuSearchTwoOptNeighborhood()
      {
         _neighborhood = new TwoOptNeighborhood();
      }

      public override IEnumerable<TOSolution> GetAllMoves(in TOSolution currentSolution)
      {
         _neighborhood.GetAllMoves(currentSolution);
         throw new System.NotImplementedException();
      }
   }
}
