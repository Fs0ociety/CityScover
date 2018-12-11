//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/12/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal class TwoOptNeighborhood : Neighborhood<ToSolution>
   {
      #region Constructors
      internal TwoOptNeighborhood()
      {
         Type = AlgorithmType.TwoOpt;
      }
      #endregion

      #region Private methods
      private CityMapGraph BuildSolutionGraph(IEnumerable<InterestPointWorker> newNodeSequence)
      {
         CityMapGraph newSolutionGraph = new CityMapGraph();
         foreach (var node in newNodeSequence)
         {
            newSolutionGraph.AddNode(node.Entity.Id, node);
         }

         // Genero gli archi dopo aver creato tutti i nodi.
         for (int i = 0; i < newNodeSequence.Count() - 1; i++)
         {
            newSolutionGraph.AddRouteFromGraph(Solver.Instance.CityMapGraph, newNodeSequence.ElementAt(i).Entity.Id, newNodeSequence.ElementAt(i + 1).Entity.Id);
         }

         // Devo fare in modo che venga costruito un ciclo, perciò aggiungo l'arco che collega l'ultimo nodo creato al primo.
         newSolutionGraph.AddRouteFromGraph(Solver.Instance.CityMapGraph, newNodeSequence.ElementAt(newNodeSequence.Count() - 1).Entity.Id, newNodeSequence.ElementAt(0).Entity.Id);
         return newSolutionGraph;
      }

      private IEnumerable<InterestPointWorker> GenerateSequence(in IEnumerable<InterestPointWorker> solutionNodes, int i, int k)
      {
         ICollection<InterestPointWorker> newSequence = new Collection<InterestPointWorker>();

         // 1. Prendi tutti gli elementi da solutionNodes[0] a solutionNodes[i-1] e aggiungili 
         // nell'ordine di processing a newSequence.
         for (int c = 0; c <= i - 1; c++)
         {
            newSequence.Add(solutionNodes.ElementAt(c));
         }

         // 2. Prendi gli elementi da solutionNodes[i] a solutionNodes[k] e aggiungili
         // in ordine inverso a newSequence.
         for (int c = k; c >= i; c--)
         {
            newSequence.Add(solutionNodes.ElementAt(c));
         }

         // 3. Prendi gli elementi da solutionNodes[k+1] all'ultimo e aggiungili
         // nell'ordine di processing a newSequence.
         for (int c = k + 1; c < solutionNodes.Count(); c++)
         {
            newSequence.Add(solutionNodes.ElementAt(c));
         }
         return newSequence;
      }
      
      private Tuple<int, int> GetMove(in ToSolution solution, int iNodeId, int kNodeId)
      {
         RouteWorker firstEdge = solution.SolutionGraph.Edges.Where(edge => edge.Entity.PointTo.Id == iNodeId).FirstOrDefault();
         RouteWorker secondEdge = solution.SolutionGraph.Edges.Where(edge => edge.Entity.PointFrom.Id == kNodeId).FirstOrDefault();
         if (firstEdge == null || secondEdge == null)
         {
            return null;
         }
         return Tuple.Create(firstEdge.Entity.Id, secondEdge.Entity.Id);
      }

      private string GetMoveDescription(in ToSolution startSolution, in ToSolution newSolution)
      {
         int firstEdgeId = newSolution.Move.Item1;
         int secondEdgeId = newSolution.Move.Item2;

         // Prendo giù i RouteWorker corrispondenti agli archi iniziali (quelli che ho tolto) 
         // dalla soluzione di partenza.
         RouteWorker firstEdge = startSolution.SolutionGraph.Edges.Where(edge => edge.Entity.Id == firstEdgeId).FirstOrDefault();
         RouteWorker secondEdge = startSolution.SolutionGraph.Edges.Where(edge => edge.Entity.Id == secondEdgeId).FirstOrDefault();

         return MessagesRepository.GetMessage(
            MessageCode.LocalSearchNewNeighborhoodMoveDetails,
            newSolution.Id,
            $"({firstEdge.Entity.PointFrom.Id}, {firstEdge.Entity.PointTo.Id})",
            $"({secondEdge.Entity.PointFrom.Id}, {secondEdge.Entity.PointTo.Id})",
            $"({firstEdge.Entity.PointTo.Id}, {secondEdge.Entity.PointTo.Id})",
            $"({firstEdge.Entity.PointFrom.Id}, {secondEdge.Entity.PointFrom.Id})");
      }
      #endregion

      #region Overrides
      internal override IEnumerable<ToSolution> GeneratingLogic(in ToSolution solution)
      {
         IEnumerable<InterestPointWorker> solutionNodes = solution.SolutionGraph.Nodes;
         ICollection<ToSolution> neighborhood = new Collection<ToSolution>();
         for (int i = 0; i < solutionNodes.Count() - 1; i++)
         {
            for (int k = i + 1; k < solutionNodes.Count(); k++)
            {
               IEnumerable<InterestPointWorker> newNodeSequence = GenerateSequence(solutionNodes, i, k);
               CityMapGraph newSolutionGraph = BuildSolutionGraph(newNodeSequence);
               Tuple<int, int> move = GetMove(solution, solutionNodes.ElementAt(i).Entity.Id, solutionNodes.ElementAt(k).Entity.Id);
               if (move is null)
               {
                  continue;
               }
               ToSolution newSolution = new ToSolution()
               {
                  SolutionGraph = newSolutionGraph,
                  Move = move                  
               };
               newSolution.Description = GetMoveDescription(solution, newSolution);
               neighborhood.Add(newSolution);
            }
         }
         return neighborhood;
      }
      #endregion
   }
}
