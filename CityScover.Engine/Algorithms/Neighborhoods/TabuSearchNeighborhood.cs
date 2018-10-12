//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 11/10/2018
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
      private IList<TabuMove> _tabuList;

      #region Constructors
      internal TabuSearchNeighborhood()
      {
      }
   
      internal TabuSearchNeighborhood(Neighborhood neighborhood, IList<TabuMove> tabuList)
      {
         _neighborhoodWorker = neighborhood ?? throw new ArgumentNullException();
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

      internal IList<TabuMove> TabuList
      {
         get => _tabuList;
         set => _tabuList = value ?? 
            throw new ArgumentNullException($"{nameof(value)} can not be null");
      }
      #endregion

      #region Internal methods
      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in TOSolution solution) =>
         _neighborhoodWorker.GetCandidates(solution);

      internal override TOSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge)
      {
         TabuMove forbiddenMove = _tabuList.
            Where(move => move.FirstEdge == currentEdge && move.SecondEdge == candidateEdge).
            FirstOrDefault();

         if (forbiddenMove != null)
         {
            // NON NECESSARIO?
            //if (_tabuList is List<TabuMove> tabuList)
            //{
            //   if (forbiddenMove.Expiration < tabuList.Capacity)
            //   {
            //      return default;
            //   }
            //}
            return default;
         }

         TOSolution solution = _neighborhoodWorker.ProcessCandidate(currentEdge, candidateEdge);
         TabuMove reversedMove = new TabuMove(currentEdge, candidateEdge, expiration: 0);
         _tabuList.Add(reversedMove);
         _tabuList.Where(move => !move.Equals(reversedMove)).ToList().
            ForEach(move => move.Expiration++);

         return solution;
      }
      #endregion
   }
}