//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 08/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class NeighborhoodFacade
   {
      private Neighborhood _neighborhood;

      #region Constructors
      internal NeighborhoodFacade(Neighborhood neighborhood)
      {
         _neighborhood = neighborhood ?? throw new ArgumentNullException(nameof(neighborhood));
      }
      #endregion

      #region Internal methods
      internal IEnumerable<TOSolution> GenerateNeighborhood(in TOSolution solution)
      {
         var candidateEdges = _neighborhood.GetCandidates(solution);
         if (candidateEdges == null)
         {
            return null;
         }

         ICollection<TOSolution> neighborhood = new Collection<TOSolution>();
         if (candidateEdges.Any())
         {
            ProcessCandidates(candidateEdges, solution, neighborhood);
         }

         return neighborhood;
      }
      #endregion
      
      #region Private methods
      private void ProcessCandidates(IDictionary<RouteWorker, IEnumerable<RouteWorker>> candidateEdges, 
         TOSolution solution, ICollection<TOSolution> neighborhood)
      {
         foreach (var currentEdge in candidateEdges.Keys)
         {
            var edges = candidateEdges[currentEdge];
            foreach (var candidateEdge in edges)
            {
               TOSolution newSolution = _neighborhood.ProcessCandidate(currentEdge, candidateEdge);
               neighborhood.Add(newSolution);
            }
         }
      }
      #endregion
   }
}
