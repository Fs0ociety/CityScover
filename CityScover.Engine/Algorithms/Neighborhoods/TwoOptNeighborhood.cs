//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 03/11/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class TwoOptNeighborhood : Neighborhood
   {
      #region Private fields
      private CityMapGraph _cityMapClone;
      private CityMapGraph _tour;
      #endregion

      #region Constructors
      internal TwoOptNeighborhood()
      {
         _cityMapClone = Solver.Instance.CityMapGraph.DeepCopy();
      }
      #endregion

      #region Private methods
      private DateTime GetTotalTime()
      {
         InterestPointWorker startPOI = _tour.GetStartPoint();
         InterestPointWorker endPOI = _tour.GetEndPoint();
         DateTime endPOITotalTime = endPOI.TotalTime;

         RouteWorker returnEdge = _cityMapClone.GetEdge(endPOI.Entity.Id, startPOI.Entity.Id);
         if (returnEdge is null)
         {
            throw new NullReferenceException(nameof(returnEdge));
         }

         double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed;
         TimeSpan timeReturn = TimeSpan.FromSeconds(returnEdge.Weight() / averageSpeedWalk);
         DateTime timeSpent = endPOITotalTime.Add(timeReturn);
         return timeSpent;
      }

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
         _tour = solution.SolutionGraph.DeepCopy();
         var currentSolutionPoints = _tour.Nodes;
         var candidateEdges = new Dictionary<RouteWorker, IEnumerable<RouteWorker>>();

         foreach (var node in currentSolutionPoints)
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
               var processingNodeId = neighbor;
               var previousProcessingNodeId = fixedNodeId;
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

      internal override TOSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge)
      {
         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _tour.DeepCopy(),
            TimeSpent = GetTotalTime()
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
      #endregion
   }
}