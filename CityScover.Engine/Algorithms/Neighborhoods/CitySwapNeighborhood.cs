//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 13/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class CitySwapNeighborhood : Neighborhood
   {
      #region Private fields
      private CityMapGraph _cityMapClone;
      private CityMapGraph _startSolutionGraph;
      #endregion

      #region Constructors
      internal CitySwapNeighborhood()
      {
         _cityMapClone = Solver.Instance.CityMapGraph;
      }
      #endregion

      #region Overrides
      internal override IDictionary<RouteWorker, IEnumerable<RouteWorker>> GetCandidates(in TOSolution solution)
      {
         // Qua non mi serve il clone perchè accedo solo in lettura. Però devo tenermi la soluzione di riferimento
         // per fare tutte le combinazioni.
         _startSolutionGraph = solution.SolutionGraph;
         var currentSolutionPoints = _startSolutionGraph.Nodes;
         var candidateEdges = new Dictionary<RouteWorker, IEnumerable<RouteWorker>>();

         foreach (var fixedNode in currentSolutionPoints)
         {
            var fixedNodeEdges = _startSolutionGraph.GetEdges(fixedNode.Entity.Id);
            // Devo guardare tutte le coppie possibili tra il nodo fisso e gli altri nodi).
            var otherNodes = currentSolutionPoints.Where(x => x.Entity.Id != fixedNode.Entity.Id);            
            foreach (var otherNode in otherNodes)
            {
               foreach (var currentEdge in fixedNodeEdges)
               {
                  var candidateEdgesCurrentEdge = new Collection<RouteWorker>();
                  RouteWorker newEdge = _cityMapClone.GetEdge(otherNode.Entity.Id, currentEdge.Entity.PointTo.Id);
                  if (newEdge == null)
                  {
                     throw new InvalidOperationException();
                  }

                  candidateEdgesCurrentEdge.Add(newEdge);
                  candidateEdges.Add(currentEdge, candidateEdgesCurrentEdge);
               }
            }
         }
         return candidateEdges;
      }

      internal override TOSolution ProcessCandidate(RouteWorker currentEdge, RouteWorker candidateEdge)
      {
         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _startSolutionGraph.DeepCopy()
         };

         newSolution.SolutionGraph.RemoveEdge(currentEdge.Entity.PointFrom.Id, currentEdge.Entity.PointTo.Id);
         newSolution.SolutionGraph.AddEdge(candidateEdge.Entity.PointFrom.Id, candidateEdge.Entity.PointTo.Id, candidateEdge);
         return newSolution;
      } 
      #endregion
   }
}
