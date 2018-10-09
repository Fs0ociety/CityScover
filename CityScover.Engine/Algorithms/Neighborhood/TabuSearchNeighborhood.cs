//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal class TabuSearchNeighborhood : Neighborhood
   {
      private Neighborhood _neighborhoodWorker;
      private IList<RouteWorker> _tabuList;

      #region Constructors
      internal TabuSearchNeighborhood(Neighborhood neighborhoodWorker)
      {
         _neighborhoodWorker = neighborhoodWorker ?? throw new ArgumentNullException();
         _tabuList = new List<RouteWorker>();
      }
      #endregion

      #region Protected properties
      protected IList<RouteWorker> TabuList => _tabuList;
      #endregion

      #region Internal methods
      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in TOSolution solution) =>
   _neighborhoodWorker.GetCandidates(solution);

      internal override TOSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge)
      {
         // TODO: Controllare che la coppia (currentEdge, candidateEdge) non sia bloccata.

         TOSolution solution = _neighborhoodWorker.ProcessCandidate(currentEdge, candidateEdge);
         return solution;
      }
      #endregion
   }
}
