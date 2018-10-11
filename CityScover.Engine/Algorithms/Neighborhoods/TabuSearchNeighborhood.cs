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

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class TabuSearchNeighborhood<T1, T2> : Neighborhood<T1, T2>
   {
      private Neighborhood<T1, T2> _neighborhoodWorker;
      private IList<RouteWorker> _tabuList;

      #region Constructors
      internal TabuSearchNeighborhood(Neighborhood<T1, T2> neighborhoodWorker)
      {
         _neighborhoodWorker = neighborhoodWorker ?? throw new ArgumentNullException();
         _tabuList = new List<RouteWorker>();
      }
      #endregion

      #region Protected properties
      protected IList<RouteWorker> TabuList => _tabuList;

      internal override ICollection<TOSolution> BuildNeighborhood(T1 currentEdge, T2 candidateEdges)
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Internal methods
      internal override IDictionary<T1, T2> GetCandidates(in TOSolution solution) =>
   _neighborhoodWorker.GetCandidates(solution);

      internal override TOSolution ProcessCandidate(T1 currentEdge, T1 candidateEdge)
      {
         // TODO: Controllare che la coppia (currentEdge, candidateEdge) non sia bloccata.

         TOSolution solution = _neighborhoodWorker.ProcessCandidate(currentEdge, candidateEdge);
         return solution;
      }
      #endregion
   }
}
