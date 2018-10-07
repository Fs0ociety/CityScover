//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 07/10/2018
//

using CityScover.Engine.Workers;
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal abstract class TabuSearchNeighborhood : INeighborhood
   {
      private IList<RouteWorker> _tabuList;

      internal TabuSearchNeighborhood()
      {
         _tabuList = new List<RouteWorker>();
      }

      protected IList<RouteWorker> TabuList => _tabuList;

      public abstract IEnumerable<TOSolution> GetAllMoves(in TOSolution currentSolution);
   }
}
