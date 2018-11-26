//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/11/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class TwoOptNeighborhood : Neighborhood
   {
      #region Private fields
      private readonly CityMapGraph _cityMap;
      private CityMapGraph _tour;
      #endregion

      #region Constructors
      internal TwoOptNeighborhood()
      {
         Type = AlgorithmType.TwoOpt;
         _cityMap = Solver.Instance.CityMapGraph;
      }
      #endregion

      #region Private methods
      private void TwoOptSwap(in RouteWorker currentEdge, RouteWorker candidateEdge,
         CityMapGraph newSolutionGraph, in int edge2PointFromId)
      {
         int currentNodeId = currentEdge.Entity.PointTo.Id;

         while (currentNodeId != edge2PointFromId)
         {
            //var currNodeAdjNode = newSolutionGraph.GetAdjacentNodes(currentNodeId)
            //   .Where(x => x != candidateEdge.Entity.PointTo.Id).FirstOrDefault();

            var currNodeAdjNode = newSolutionGraph
               .GetAdjacentNodes(currentNodeId)
               .FirstOrDefault(x => x != candidateEdge.Entity.PointTo.Id);

            if (currNodeAdjNode == 0)
            {
               throw new InvalidOperationException();
            }

            newSolutionGraph.RemoveEdge(currentNodeId, currNodeAdjNode);
            newSolutionGraph.AddRouteFromGraph(_cityMap, currNodeAdjNode, currentNodeId);
            currentNodeId = currNodeAdjNode;
         }
      }
      #endregion

      #region Internal methods

      #region GetCandidates [Single-Threaded]
      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in TOSolution solution)
      {
         _tour = solution.SolutionGraph.DeepCopy();
         var candidateEdges = new Dictionary<RouteWorker, IEnumerable<RouteWorker>>();

         foreach (var node in _tour.Nodes)
         {
            int fixedNodeId = node.Entity.Id;
            var itemNeighbors = _tour.GetAdjacentNodes(fixedNodeId);

            foreach (var neighbor in itemNeighbors)
            {
               var candidateEdgesCurrentEdge = new Collection<RouteWorker>();
               var currentEdge = _tour.GetEdge(fixedNodeId, neighbor);
               if (currentEdge is null)
               {
                  continue;
               }

               currentEdge.IsVisited = true;
               int processingNodeId = neighbor;
               int previousProcessingNodeId = fixedNodeId;
               int newProcessingNodeId = default;

               while (processingNodeId != fixedNodeId)
               {
                  var nextNeighbors = _tour.GetAdjacentNodes(processingNodeId);

                  foreach (var adjacentNodeId in nextNeighbors)
                  {
                     var procNodeAdjNodeEdge = _tour.GetEdge(processingNodeId, adjacentNodeId);
                     if (procNodeAdjNodeEdge is null)
                     {
                        continue;
                     }

                     if (!_tour.AreAdjacentEdges(
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
      #endregion

      #region GetCandidates [Multi-Threaded]
      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidatesParallel(in TOSolution solution)
      {
         _tour = solution.SolutionGraph.DeepCopy();
         var candidateEdges = new ConcurrentDictionary<RouteWorker, IEnumerable<RouteWorker>>();

         Parallel.ForEach(_tour.Nodes, node =>
         {
            int fixedNodeId = node.Entity.Id;
            var itemNeighbors = _tour.GetAdjacentNodes(fixedNodeId);

            foreach (var neighbor in itemNeighbors)
            {
               var candidateEdgesCurrentEdge = new Collection<RouteWorker>();
               var currentEdge = _tour.GetEdge(fixedNodeId, neighbor);

               if (currentEdge is null)
               {
                  continue;
               }
               currentEdge.IsVisited = true;
               int processingNodeId = neighbor;
               int previousProcessingNodeId = fixedNodeId;
               int newProcessingNodeId = default;

               while (processingNodeId != fixedNodeId)
               {
                  var nextNeighbors = _tour.GetAdjacentNodes(processingNodeId);

                  foreach (var adjacentNodeId in nextNeighbors)
                  {
                     var procNodeAdjNodeEdge = _tour.GetEdge(processingNodeId, adjacentNodeId);
                     if (procNodeAdjNodeEdge is null)
                     {
                        continue;
                     }

                     if (!_tour.AreAdjacentEdges(currentEdge.Entity.PointFrom.Id, currentEdge.Entity.PointTo.Id, 
                        procNodeAdjNodeEdge.Entity.PointFrom.Id, procNodeAdjNodeEdge.Entity.PointTo.Id))
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
               candidateEdges.TryAdd(currentEdge, candidateEdgesCurrentEdge);
            }
         });

         return candidateEdges;
      }
      #endregion
   
      internal override TOSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge)
      {
         CityMapGraph newSolutionGraph = _tour.DeepCopy();

         int currentEdgePointFromId = currentEdge.Entity.PointFrom.Id;
         int currentEdgePointToId = currentEdge.Entity.PointTo.Id;
         int candidateEdgePointFromId = candidateEdge.Entity.PointFrom.Id;
         int candidateEdgePointToId = candidateEdge.Entity.PointTo.Id;

         newSolutionGraph.RemoveEdge(currentEdgePointFromId, currentEdgePointToId);
         newSolutionGraph.RemoveEdge(candidateEdgePointFromId, candidateEdgePointToId);

         newSolutionGraph.AddRouteFromGraph(_cityMap, currentEdgePointFromId, candidateEdgePointFromId);
         newSolutionGraph.AddRouteFromGraph(_cityMap, currentEdgePointToId, candidateEdgePointToId);

         // Nota: Affinchè l'algoritmo di merda della Nonato funzioni, dobbiamo cambiare il verso di diversi altri archi.
         TwoOptSwap(currentEdge, candidateEdge, newSolutionGraph, candidateEdgePointFromId);

         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = newSolutionGraph            
         };

         return newSolution;
      }
      #endregion
   }
}