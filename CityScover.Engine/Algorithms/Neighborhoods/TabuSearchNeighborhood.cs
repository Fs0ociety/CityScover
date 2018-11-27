//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/11/2018
//

using CityScover.Engine.Algorithms.Metaheuristics;
using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class TabuSearchNeighborhood : Neighborhood
   {
      private Neighborhood _neighborhoodWorker;
      private readonly IList<TabuMove> _tabuList;

      #region Constructors
      internal TabuSearchNeighborhood()
      {
         _tabuList = new List<TabuMove>();
      }

      internal TabuSearchNeighborhood(Neighborhood neighborhood, IList<TabuMove> tabuList)
      {
         _neighborhoodWorker = neighborhood ?? throw new ArgumentNullException();
         Type = neighborhood.Type;
         _tabuList = tabuList;

      }
      #endregion

      #region Internal properties
      internal Neighborhood NeighborhoodWorker
      {
         get => _neighborhoodWorker;
         set => _neighborhoodWorker = value ?? 
            throw new ArgumentNullException($"{nameof(value)} can not be null");
      }

      internal IList<TabuMove> TabuList => _tabuList;
      #endregion

      #region Internal methods
      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in ToSolution solution) =>
         _neighborhoodWorker.GetCandidates(solution);

      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidatesParallel(in ToSolution solution) => 
         _neighborhoodWorker.GetCandidatesParallel(solution);

      internal override ToSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge)
      {
         TabuMove forbiddenMove = _tabuList
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