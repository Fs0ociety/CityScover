//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 06/12/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CityScover.Engine.Algorithms.LocalSearches;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class NeighborhoodFacade
   {
      private readonly Neighborhood _neighborhood;
      private readonly LocalSearchTemplate _algorithm;

      #region Constructors
      internal NeighborhoodFacade(Neighborhood neighborhood, LocalSearchTemplate algorithm)
      {
         _neighborhood = neighborhood ?? throw new ArgumentNullException(nameof(neighborhood));
         _algorithm = algorithm;
      }
      #endregion

      #region Private methods
      private void ProcessCandidates(
         IDictionary<RouteWorker, IEnumerable<RouteWorker>> candidateEdges, 
         ICollection<ToSolution> neighborhood)
      {
         foreach (var currentEdge in candidateEdges.Keys)
         {
            var edges = candidateEdges[currentEdge];
            foreach (var candidateEdge in edges)
            {
               ToSolution newSolution = _neighborhood.ProcessCandidate(currentEdge, candidateEdge);
               if (newSolution != null)
               {
                  neighborhood.Add(newSolution);
               }
            }
         }
      }
      #endregion

      #region Internal types
      internal enum RunningMode
      {
         Sequential,
         Parallel
      }
      #endregion

      #region Internal methods
      internal IEnumerable<ToSolution> GenerateNeighborhood(in ToSolution solution, RunningMode runningMode = RunningMode.Sequential)
      {
         _algorithm.SendMessage(MessageCode.LSNewNeighborhood, _algorithm.CurrentStep);

         var candidateEdges = runningMode == RunningMode.Parallel 
            ? _neighborhood.GetCandidatesParallel(solution) 
            : _neighborhood.GetCandidates(solution);

         if (candidateEdges is null)
         {
            return null;
         }

         if (!candidateEdges.Any())
         {
            return null;
         }

         ICollection<ToSolution> neighborhood = new Collection<ToSolution>();
         ProcessCandidates(candidateEdges, neighborhood);
         return neighborhood;
      }
      #endregion
   }
}
