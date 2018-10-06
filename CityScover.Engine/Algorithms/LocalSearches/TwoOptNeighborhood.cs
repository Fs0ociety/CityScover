//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 05/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.LocalSearches
{
   internal class TwoOptNeighborhood : INeighborhood
   {
      private CityMapGraph _currentSolutionGraph;
      private CityMapGraph _cityMapClone;

      internal IEnumerable<TOSolution> GetAllMoves(in TOSolution currentSolution)
      {
         _cityMapClone = Solver.Instance.CityMapGraph.DeepCopy();
         var neighborhood = new Collection<TOSolution>();

         _currentSolutionGraph = currentSolution.SolutionGraph.DeepCopy();
         var currentSolutionPoints = _currentSolutionGraph.Nodes;

         foreach (var node in currentSolutionPoints)
         {
            int fixedNodeId = node.Entity.Id;
            var itemNeighbors = _currentSolutionGraph.GetAdjacentNodes(fixedNodeId);

            foreach (var neighbor in itemNeighbors)
            {
               var candidateEdges = new Collection<RouteWorker>();
               var currentEdge = _currentSolutionGraph.GetEdge(fixedNodeId, neighbor);
               if (currentEdge == null)
               {
                  continue;
               }

               currentEdge.IsVisited = true;
               var processingNodeId = neighbor;
               var previousProcessingNodeId = fixedNodeId;
               int newProcessingNodeId = default;

               while (processingNodeId != fixedNodeId)
               {
                  var nextNeighbors = _currentSolutionGraph.GetAdjacentNodes(processingNodeId);

                  foreach (var adjacentNodeId in nextNeighbors)
                  {
                     var procNodeAdjNodeEdge = _currentSolutionGraph.GetEdge(processingNodeId, adjacentNodeId);
                     if (procNodeAdjNodeEdge == null)
                     {
                        continue;
                     }

                     if (!_currentSolutionGraph.AreAdjacentEdges(
                           currentEdge.Entity.PointFrom.Id,
                           currentEdge.Entity.PointTo.Id,
                           procNodeAdjNodeEdge.Entity.PointFrom.Id,
                           procNodeAdjNodeEdge.Entity.PointTo.Id))
                     {
                        candidateEdges.Add(procNodeAdjNodeEdge);
                     }

                     procNodeAdjNodeEdge.IsVisited = true;

                     if (adjacentNodeId != previousProcessingNodeId)
                     {
                        previousProcessingNodeId = processingNodeId;
                        newProcessingNodeId = adjacentNodeId;
                     }
                  }
                  processingNodeId = newProcessingNodeId;
               }

               if (candidateEdges.Any())
               {
                  ProcessingCandidates(candidateEdges, currentEdge, neighborhood);
               }
            }
         }
         return neighborhood;
      }

      private void ProcessingCandidates(in Collection<RouteWorker> candidateEdges, in RouteWorker currentEdge, in Collection<TOSolution> neighborhood)
      {
         foreach (var candidateEdge in candidateEdges)
         {
            TOSolution newSolution = new TOSolution()
            {
               SolutionGraph = _currentSolutionGraph.DeepCopy()
            };

            int currentEdgePointFromId = currentEdge.Entity.PointFrom.Id;
            int currentEdgePointToId = currentEdge.Entity.PointTo.Id;
            int candidateEdgePointFromId = candidateEdge.Entity.PointFrom.Id;
            int candidateEdgePointToId = candidateEdge.Entity.PointTo.Id;

            newSolution.SolutionGraph.RemoveEdge(currentEdgePointFromId, currentEdgePointToId);
            newSolution.SolutionGraph.RemoveEdge(candidateEdgePointFromId, candidateEdgePointToId);

            newSolution.SolutionGraph.AddRouteFromGraph(_cityMapClone, currentEdgePointFromId, candidateEdgePointFromId);
            newSolution.SolutionGraph.AddRouteFromGraph(_cityMapClone, currentEdgePointToId, candidateEdgePointToId);

            //Nota: Affinchè l'algoritmo di merda della Nonato funzioni, dobbiamo cambiare il verso di diversi altri archi.
            TwoOptTourInversion(currentEdge, candidateEdge, newSolution, candidateEdgePointFromId);
            neighborhood.Add(newSolution);
         }
      }

      private void TwoOptTourInversion(in RouteWorker currentEdge, RouteWorker candidateEdge, TOSolution newSolution, in int edge2PointFromId)
      {
         int currentNodeId = currentEdge.Entity.PointTo.Id;

         while (currentNodeId != edge2PointFromId)
         {
            var currNodeAdjNode = newSolution.SolutionGraph.GetAdjacentNodes(currentNodeId)
               .Where(x => x != candidateEdge.Entity.PointTo.Id).FirstOrDefault();

            if (currNodeAdjNode == 0)
            {
               throw new InvalidOperationException();
            }

            newSolution.SolutionGraph.RemoveEdge(currentNodeId, currNodeAdjNode);
            newSolution.SolutionGraph.AddRouteFromGraph(_cityMapClone, currNodeAdjNode, currentNodeId);
            currentNodeId = currNodeAdjNode;
         }
      }

      //IEnumerable<TOSolution> GetAllMoves(in TOSolution currentSolution, IList<int> tabuList)
      //{
      //   return GetAllMoves(currentSolution);
      //}

      IEnumerable<TOSolution> INeighborhood.GetAllMoves(in TOSolution currentSolution)
      {
         throw new NotImplementedException();
      }
   }
}
