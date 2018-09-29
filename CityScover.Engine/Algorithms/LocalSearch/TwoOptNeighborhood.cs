//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 28/09/2018
//

using CityScover.Engine.Workers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine.Algorithms.LocalSearch
{
   internal class TwoOptNeighborhood : Neighborhood
   {
      internal override IEnumerable<TOSolution> GetAllMoves(TOSolution currentSolution)
      {
         var neighborhood = new Collection<TOSolution>();

         var currentSolutionGraph = currentSolution.SolutionGraph;
         var currentSolutionPoints = currentSolutionGraph.Nodes;
         foreach (var item in currentSolutionPoints)
         {
            var fixedNode = item;
            var itemNeighbors = currentSolutionGraph.GetAdjacentNodes(fixedNode.Entity.Id);

            foreach (var neighbor in itemNeighbors)
            {
               var candidateEdges = new Collection<RouteWorker>();
               var currentEdge = currentSolutionGraph.GetEdge(fixedNode.Entity.Id, neighbor);
               if (currentEdge == null)
               {
                  continue;
               }

               currentEdge.IsVisited = true;
               var processingNodeId = neighbor;
               var previousProcessingNodeId = fixedNode.Entity.Id;
               int newProcessingNodeId = default;

               while (processingNodeId != previousProcessingNodeId)
               {
                  var nextNeighbors = currentSolutionGraph.GetAdjacentNodes(processingNodeId);

                  foreach (var adjacentNodeId in nextNeighbors)
                  {
                     var procNodeAdjNodeEdge = currentSolutionGraph.GetEdge(processingNodeId, adjacentNodeId);
                     if (procNodeAdjNodeEdge == null)
                     {
                        continue;
                     }

                     // TODO: Da implementare funzione su libreria dei grafi che controlla se due archi sono adiacenti.
                     //if(!currentSolutionGraph.AreAdjacentEdges(currentEdge, procNodeAdjNodeEdge))
                     //{
                     //   candidateEdges.Add(procNodeAdjNodeEdge);
                     //}

                     procNodeAdjNodeEdge.IsVisited = true;

                     // TODO: questione grafo non orientato.
                     var adjNodeProcNodeEdge = currentSolutionGraph.GetEdge(adjacentNodeId, processingNodeId);
                     if (adjNodeProcNodeEdge == null)
                     {
                        continue;
                     }

                     adjNodeProcNodeEdge.IsVisited = true;

                     if (adjacentNodeId != previousProcessingNodeId)
                     {
                        previousProcessingNodeId = processingNodeId;
                        newProcessingNodeId = adjacentNodeId;
                     }
                  }
                  processingNodeId = newProcessingNodeId;
               }
               ProcessingCandidates(candidateEdges, currentEdge, neighborhood);
            }
         }
         return neighborhood;
      }

      private void ProcessingCandidates(Collection<RouteWorker> candidateEdges, RouteWorker currentEdge, Collection<TOSolution> neighborhood)
      {
         var currentEdgeCost = currentEdge.Weight;
         foreach (var edge in candidateEdges)
         {
            // TODO: fare un clone per ogni candidato, dove ci tolgo e aggiungo gli archi.
            //var newSolution = currentSolutionGraph.DeepCopy();
            //neighborhood.Add(newSolution);
         }
      }
   }
}
