//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 08/10/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class NeighborhoodFacade<T1, T2>
   {
      private Neighborhood<T1, T2> _neighborhood;

      #region Constructors
      internal NeighborhoodFacade(Neighborhood<T1, T2> neighborhood)
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
         
         if (candidateEdges.Any())
         {
            return ProcessCandidates(candidateEdges, solution);
         }

         return Enumerable.Empty<TOSolution>();
      }
      #endregion
      
      #region Private methods
      private IEnumerable<TOSolution> ProcessCandidates(IDictionary<T1, T2> candidateEdges, TOSolution solution)
      {
         ICollection<TOSolution> neighborhood = new Collection<TOSolution>();
         foreach (var currentEdge in candidateEdges.Keys)
         {
            var edges = candidateEdges[currentEdge];
            //foreach (var candidateEdge in edges)
            //{
            //   TOSolution newSolution = _neighborhood.ProcessCandidate(currentEdge, candidateEdge);               
            //   neighborhood.Add(newSolution);
            //}
            ICollection<TOSolution> currentEdgeSolutions = _neighborhood.BuildNeighborhood(currentEdge, edges);
            neighborhood.Concat(currentEdgeSolutions);
         }
         return neighborhood;
      }
      #endregion
   }
}
