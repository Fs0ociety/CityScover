//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 12/10/2018
//

using CityScover.Engine.Workers;
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal abstract class Neighborhood
   {
      #region Internal abstract methods
      /// <summary>
      /// Returns a map formed with a edge and a list of candidates edges.
      /// </summary>
      /// <param name="solution"> Solution to process. </param>
      /// <returns></returns>
      internal abstract IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in TOSolution solution);

      /// <summary>
      /// Returns a new TOSolution type formed with the new processed informations.
      /// </summary>
      /// <param name="currentEdge"></param>
      /// <param name="candidateEdge"></param>
      /// <returns></returns>
      internal abstract TOSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge);
      #endregion
   }
}