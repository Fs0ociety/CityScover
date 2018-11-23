//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 23/11/2018
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
      private LocalSearchTemplate _algorithm;

      #region Constructors
      internal NeighborhoodFacade(Neighborhood neighborhood, LocalSearchTemplate algorithm)
      {
         _neighborhood = neighborhood ?? throw new ArgumentNullException(nameof(neighborhood));
         _algorithm = algorithm;
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
               if (newSolution != null)
               {
                  _algorithm.Move = Tuple.Create(currentEdge.Entity.Id, candidateEdge.Entity.Id);
                  string message = MessagesRepository.GetMessage(
                     MessageCode.LSNewNeighborhoodMoveDetails,
                     newSolution.Id,
                     $"({currentEdge.Entity.PointFrom.Id}, {currentEdge.Entity.PointTo.Id})",
                     $"({candidateEdge.Entity.PointFrom.Id}, {candidateEdge.Entity.PointTo.Id})");

                  newSolution.Description = message;
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
      internal IEnumerable<TOSolution> GenerateNeighborhood(in TOSolution solution, RunningMode runningMode = RunningMode.Sequential)
      {
         _algorithm.SendMessage(MessageCode.LSNewNeighborhood, _algorithm.CurrentStep);
         ICollection<TOSolution> neighborhood = default;
         IDictionary<RouteWorker, IEnumerable<RouteWorker>> candidateEdges = default;

         if (runningMode == RunningMode.Parallel)
         {
            candidateEdges = _neighborhood.GetCandidatesParallel(solution);
         }
         else
         {
            candidateEdges = _neighborhood.GetCandidates(solution);
         }

         if (candidateEdges is null)
         {
            return neighborhood;
         }
         if (candidateEdges.Any())
         {
            neighborhood = new Collection<TOSolution>();
            ProcessCandidates(candidateEdges, solution, neighborhood);
         }

         return neighborhood;
      }
      #endregion
   }
}
