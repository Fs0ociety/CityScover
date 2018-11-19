//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 19/11/2018
//

using CityScover.Engine.Workers;
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal abstract class Neighborhood
   {
      #region Internal properties
      internal AlgorithmType Type { get; set; }
      #endregion

      #region Internal abstract methods
      /// <summary>
      /// Returns a map formed with a edge and a list of candidates edges.
      /// </summary>
      /// <param name="solution"> Solution to process. </param>
      /// <returns></returns>
      internal abstract IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in TOSolution solution);
      internal abstract IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidatesParallel(in TOSolution solution);

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