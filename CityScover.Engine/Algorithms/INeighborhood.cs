//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 06/10/2018
//

using System.Collections.Generic;

namespace CityScover.Engine.Algorithms
{
   internal interface INeighborhood
   {
      IEnumerable<TOSolution> GetAllMoves(in TOSolution currentSolution);
   }   
}
