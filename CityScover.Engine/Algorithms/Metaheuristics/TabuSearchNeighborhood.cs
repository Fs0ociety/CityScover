//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 06/10/2018
//

using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal abstract class TabuSearchNeighborhood : INeighborhood
   {
      protected IList<int> _tabuList;

      internal TabuSearchNeighborhood()
      {
         _tabuList = new List<int>();
      }

      public abstract IEnumerable<TOSolution> GetAllMoves(in TOSolution currentSolution);
   }
}
