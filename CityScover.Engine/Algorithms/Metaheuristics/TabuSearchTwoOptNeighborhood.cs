//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 07/10/2018
//

using CityScover.Engine.Algorithms.LocalSearches;
using CityScover.Engine.Workers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal class TabuSearchTwoOptNeighborhood : TabuSearchNeighborhood
   {
      private CityMapGraph _currentSolutionGraph;
      private CityMapGraph _cityMapClone;
      private TwoOptNeighborhood _neighborhood;

      internal TabuSearchTwoOptNeighborhood()
      {
         _neighborhood = new TwoOptNeighborhood();
      }

      public override IEnumerable<TOSolution> GetAllMoves(in TOSolution currentSolution)
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
               var currentEdge = _currentSolutionGraph.GetEdge(fixedNodeId, neighbor);
               if (currentEdge == null)
               {
                  continue;
               }

               var candidateEdges = new Collection<RouteWorker>();
               _neighborhood.SetCandidates(candidateEdges, currentEdge, fixedNodeId, neighbor);

               // Filtrare per archi non utilizzabili perchè bloccati in qualche modo dalla Tabu List.
               // Dopo aver eseguito questo bellissimo filtro (metodo o LINQ), dentro la processingCandidates andranno
               // a finire solo quelli non bloccati dalla tabu list.
               var allowedEdges = (Collection<RouteWorker>)candidateEdges.Where(edge => !TabuList.Contains(edge));

               if (allowedEdges.Any())
               {
                  _neighborhood.ProcessingCandidates(allowedEdges, currentEdge, neighborhood);
               }
                              
               // Incremento expiration deu tabu list items.
            }
         }

         return neighborhood;
      }
   }
}
