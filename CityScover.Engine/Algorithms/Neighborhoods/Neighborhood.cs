//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/10/2018
//

using CityScover.Engine.Workers;
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal abstract class Neighborhood
   {
      #region Internal abstract methods
      internal abstract IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in TOSolution solution);
      internal abstract TOSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge);
      #endregion
   }
}