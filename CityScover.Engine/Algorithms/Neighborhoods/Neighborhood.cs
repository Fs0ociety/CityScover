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
   internal abstract class Neighborhood<T1, T2>
   {
      #region Internal abstract methods
      internal abstract IDictionary<T1, T2> GetCandidates(in TOSolution solution);
      //internal abstract IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in TOSolution solution);
      //internal abstract TOSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge);      
      internal abstract TOSolution ProcessCandidate(T1 currentEdge, T1 candidateEdge);

      internal abstract ICollection<TOSolution> BuildNeighborhood(T1 currentEdge, T2 candidateEdges);
      #endregion
   }
}