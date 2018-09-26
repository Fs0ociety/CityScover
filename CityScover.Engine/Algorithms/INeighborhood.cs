//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 26/09/2018
//

using System.Collections.Generic;

namespace CityScover.Engine.Algorithms
{
   internal interface INeighborhood
   {
      IEnumerable<TOSolution> GetAllMoves(TOSolution currentSolution);   
   }   
}
