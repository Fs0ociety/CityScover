using System;
using System.Collections.Generic;
using System.Linq;
using CityScover.Engine.Algorithms.Metaheuristics;
using CityScover.Engine.Workers;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class TabuSearchNeighborhood2 : Neighborhood
   {
      private Neighborhood _neighborhoodWorker;

      #region Constructors
      internal TabuSearchNeighborhood2() { }

      internal TabuSearchNeighborhood2(Neighborhood neighborhood)
      {
         _neighborhoodWorker = neighborhood ?? throw new ArgumentNullException();
         Type = neighborhood.Type;
      }
      #endregion

      #region Internal properties
      internal Neighborhood NeighborhoodWorker
      {
         get => _neighborhoodWorker;
         set => _neighborhoodWorker = value ?? 
            throw new ArgumentNullException($"{nameof(value)} can not be null");
      }

      internal TabuSearch2 Algorithm
      {
         get;
         set;
      }
      #endregion

      #region Overrides
      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in ToSolution solution) =>
         _neighborhoodWorker.GetCandidates(solution);

      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidatesParallel(in ToSolution solution) =>
         _neighborhoodWorker.GetCandidatesParallel(solution);

      internal override ToSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge)
      {
         TabuMove forbiddenMove = Algorithm.TabuList
            .FirstOrDefault(move => move.FirstEdgeId == currentEdge.Entity.Id && 
                                    move.SecondEdgeId == candidateEdge.Entity.Id);

         if (forbiddenMove != null)
         {
            return default;
         }

         var solution = _neighborhoodWorker.ProcessCandidate(currentEdge, candidateEdge);
         return solution;
      }
      #endregion
   }
}
