//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 26/09/2018
//

using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.LocalSearch
{
   internal class TwoOptNeighborhood : INeighborhood
   {
      IEnumerable<TOSolution> INeighborhood.GetAllMoves(TOSolution currentSolution)
      {
         throw new System.NotImplementedException();
      }
   }
}
