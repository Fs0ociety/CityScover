//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 01/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.LocalSearch
{
   internal class TwoOptNeighborhood : Neighborhood
   {
      private CityMapGraph _currentSolutionGraph;
      private CityMapGraph _cityMapClone;

      internal override IEnumerable<TOSolution> GetAllMoves(in TOSolution currentSolution)
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

               if (candidateEdges.Count() > 0)
               {
                  ProcessingCandidates(candidateEdges, currentEdge, neighborhood);
               }               
            }
         }
         return neighborhood;
      }

      private void ProcessingCandidates(in Collection<RouteWorker> candidateEdges, in RouteWorker currentEdge, in Collection<TOSolution> neighborhood)
      {
         foreach (var edge in candidateEdges)
         {
            var newSolution = new TOSolution()
            {
               SolutionGraph = _currentSolutionGraph.DeepCopy()
            };

            int edge1PointFromId = currentEdge.Entity.PointFrom.Id;
            int edge1PointToId = currentEdge.Entity.PointTo.Id;
            int edge2PointFromId = edge.Entity.PointFrom.Id;
            int edge2PointToId = edge.Entity.PointTo.Id;

            newSolution.SolutionGraph.RemoveEdge(edge1PointFromId, edge1PointToId);
            newSolution.SolutionGraph.RemoveEdge(edge2PointFromId, edge2PointToId);

            RouteWorker newEdge1 = _cityMapClone.GetEdge(edge1PointFromId, edge2PointFromId);
            if (newEdge1 == null)
            {
               throw new InvalidOperationException();
            }

            RouteWorker newEdge2 = _cityMapClone.GetEdge(edge1PointToId, edge2PointToId);
            if (newEdge2 == null)
            {
               throw new InvalidOperationException();
            }

            newSolution.SolutionGraph.AddEdge(edge1PointFromId, edge2PointFromId, newEdge1);
            newSolution.SolutionGraph.AddEdge(edge1PointToId, edge2PointToId, newEdge2);

            //Nota: affinchè l'algoritmo di merda della Nonato funzioni, dobbiamo cambiare il verso di diversi altri archi.            
            TwoOptTourInversion(currentEdge, edge, newSolution, edge2PointFromId);

            neighborhood.Add(newSolution);
         }
      }

      private void TwoOptTourInversion(in RouteWorker currentEdge, in RouteWorker candidateEdge, TOSolution newSolution, in int edge2PointFromId)
      {
         int currentNodeId = currentEdge.Entity.PointTo.Id;
         while (currentNodeId != edge2PointFromId)
         {
            var currNodeAdjNode = newSolution.SolutionGraph.GetAdjacentNodes(currentNodeId).Where(x => x != candidateEdge.Entity.PointTo.Id).FirstOrDefault();
            if (currNodeAdjNode == null)
            {
               throw new InvalidOperationException();
            }

            newSolution.SolutionGraph.RemoveEdge(currentNodeId, currNodeAdjNode);

            RouteWorker invertedEdge = newSolution.SolutionGraph.GetEdge(currNodeAdjNode, currentNodeId);
            newSolution.SolutionGraph.AddEdge(currNodeAdjNode, currentNodeId, invertedEdge);
            currentNodeId = currNodeAdjNode;
         }
      }
   }
}
