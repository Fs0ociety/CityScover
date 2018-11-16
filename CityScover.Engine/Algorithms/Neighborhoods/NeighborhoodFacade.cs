//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/11/2018
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
      #region Private fields
      private Neighborhood _neighborhood;
      private LocalSearchTemplate _algorithm;
      #endregion

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

      #region Internal methods
      internal IEnumerable<TOSolution> GenerateNeighborhood(in TOSolution solution)
      {
         _algorithm.SendMessage(MessageCode.LSNewNeighborhood, _algorithm.CurrentStep + 1);
         ICollection<TOSolution> neighborhood = default;
         var candidateEdges = _neighborhood.GetCandidates(solution);
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
