//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 23/09/2018
//

using System.Collections.Generic;

namespace CityScover.Engine.Algorithms
{
   internal abstract class TONeighborhood
   {
      internal TOSolution GetBest(IEnumerable<TOSolution> neighborhood, TOSolution currentSolution, byte numImprovements = 0)
      {
         return null;
      }

      internal abstract IEnumerable<TOSolution> GetAllMoves(TOSolution currentSolution);   
   }
}
