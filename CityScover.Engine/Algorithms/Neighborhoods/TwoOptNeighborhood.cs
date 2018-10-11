//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class TwoOptNeighborhood : Neighborhood<RouteWorker, IEnumerable<RouteWorker>>
   {
      private CityMapGraph _cityMapClone;
      private CityMapGraph _currentSolutionGraph;

      #region Constructors
      internal TwoOptNeighborhood()
      {
         _cityMapClone = Solver.Instance.CityMapGraph.DeepCopy();
      }
      #endregion

      #region Private methods
      private void TwoOptSwap(in RouteWorker currentEdge, RouteWorker candidateEdge, TOSolution newSolution, in int edge2PointFromId)
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
      #endregion

      #region Internal methods
      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in TOSolution solution)
      {
         _currentSolutionGraph = solution.SolutionGraph.DeepCopy();
         var currentSolutionPoints = _currentSolutionGraph.Nodes;
         var candidateEdges = new Dictionary<RouteWorker, IEnumerable<RouteWorker>>();

         foreach (var node in currentSolutionPoints)
         {
            int fixedNodeId = node.Entity.Id;
            var itemNeighbors = _currentSolutionGraph.GetAdjacentNodes(fixedNodeId);

            foreach (var neighbor in itemNeighbors)
            {
               var candidateEdgesCurrentEdge = new Collection<RouteWorker>();
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
                        candidateEdgesCurrentEdge.Add(procNodeAdjNodeEdge);
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

               candidateEdges.Add(currentEdge, candidateEdgesCurrentEdge);
            }
         }

         return candidateEdges;
      }

      internal override TOSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge)
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

         // Nota: Affinchè l'algoritmo di merda della Nonato funzioni, dobbiamo cambiare il verso di diversi altri archi.
         TwoOptSwap(currentEdge, candidateEdge, newSolution, candidateEdgePointFromId);

         return newSolution;
      }

      internal override ICollection<TOSolution> BuildNeighborhood(RouteWorker currentEdge, IEnumerable<RouteWorker> candidateEdges)
      {
         ICollection<TOSolution> solutions = new Collection<TOSolution>();
         foreach (var candidateEdge in candidateEdges)
         {
            TOSolution newSolution = ProcessCandidate(currentEdge, candidateEdge);
            solutions.Add(newSolution);
         }
         return solutions;
      }
      #endregion
   }
}