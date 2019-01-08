//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 03/01/2019
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.VariableDepthSearch
{
   internal class LinKernighan : Algorithm
   {
      #region Private fields
      private ICollection<RouteWorker> _executedMoves;      
      private ICollection<ToSolution> _solutionsHistory;
      #endregion

      #region Constructors
      internal LinKernighan() : this(null)
      {         
      }

      internal LinKernighan(AlgorithmTracker provider) : base(provider)
         => Type = AlgorithmType.LinKernighan;
      #endregion

      #region Internal properties
      internal ToSolution CurrentBestSolution { get; set; }
      //internal ToSolution CurrentBestStepSolution { get; set; }
      internal int MaxSteps { get; set; }
      #endregion

      #region Private methods
      private InterestPointWorker GetClosestSNeighbor(CityMapGraph workingGraph, int jNodeId, int iNodeId)
      {
         // Mi ricavo il predecessore di j.
         int predecessorOfJNodeId = workingGraph.GetPredecessorNodes(jNodeId).FirstOrDefault();
         if (predecessorOfJNodeId == 0)
         {
            return null;
         }

         // Mi ricavo l'insieme dei nodi del grafo corrente della soluzione, che esclude i nodi i e il
         // predecessore di j (per sicurezza anche j stesso).
         InterestPointWorker predecessorJNode = workingGraph[predecessorOfJNodeId];
         InterestPointWorker iNode = workingGraph[iNodeId];
         InterestPointWorker jNode = workingGraph[jNodeId];

         InterestPointWorker[] notPermittedNodes = { iNode, jNode, predecessorJNode };
         IEnumerable<InterestPointWorker> permittedNodes = workingGraph.Nodes.Except(notPermittedNodes);
         if (!permittedNodes.Any())
         {
            return null;
         }

         var sCandidateEdges = Solver.CityMapGraph.Edges.Where(edge => permittedNodes.Where(
            node => node.Entity.Id == edge.Entity.PointTo.Id &&
            edge.Entity.PointFrom.Id == jNodeId).Any());

         var sCandidateEdge = sCandidateEdges.OrderBy(edge => edge.Weight.Invoke()).FirstOrDefault();

         if (sCandidateEdge is null)
         {
            return null;
         }

         return workingGraph[sCandidateEdge.Entity.PointTo.Id];
      }

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
            newSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, newNodeSequence.ElementAt(i).Entity.Id, newNodeSequence.ElementAt(i + 1).Entity.Id);
         }

         // Devo fare in modo che venga costruito un ciclo, perciò aggiungo l'arco che collega l'ultimo nodo creato al primo.
         newSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, newNodeSequence.ElementAt(newNodeSequence.Count() - 1).Entity.Id, newNodeSequence.ElementAt(0).Entity.Id);
         return newSolutionGraph;
      }

      private IEnumerable<InterestPointWorker> GenerateSequence(in IEnumerable<InterestPointWorker> solutionNodes, int i, int s)
      {
         ICollection<InterestPointWorker> newSequence = new Collection<InterestPointWorker>();

         if (s < i)
         {
            // 1. Mi copio in parte tutti gli elementi prima di s, più s compreso.
            Collection<InterestPointWorker> beforeSNodes = new Collection<InterestPointWorker>();
            for (int c = 0; c <= s; c++)
            {
               beforeSNodes.Add(solutionNodes.ElementAt(c));
            }

            // 2. Prendi gli elementi da solutionNodes[s+1] a solutionNodes[i] escluso, e aggiungili
            // nell'ordine di processing a newSequence.
            for (int c = s + 1; c < i; c++)
            {
               newSequence.Add(solutionNodes.ElementAt(c));
            }

            // 3. Inserisco in ordine inverso gli elementi tenuti in parte prima.
            for (int c = beforeSNodes.Count() - 1; c >= 0; c--)
            {
               newSequence.Add(beforeSNodes.ElementAt(c));
            }

            // 4. Prendi gli elementi da solutionNodes[i] a solutionNodes[solutionCount - 1] e aggiungili
            // in ordine inverso a newSequence.
            for (int c = solutionNodes.Count() - 1; c >= i; c--)
            {
               newSequence.Add(solutionNodes.ElementAt(c));
            }
         }
         else
         {
            // 1. Prendi tutti gli elementi da solutionNodes[0] a solutionNodes[i-1] e aggiungili 
            // nell'ordine di processing a newSequence.
            for (int c = 0; c <= i - 1; c++)
            {
               newSequence.Add(solutionNodes.ElementAt(c));
            }

            // 2. Prendi gli elementi da solutionNodes[i] a solutionNodes[s] e aggiungili
            // in ordine inverso a newSequence.
            for (int c = s; c >= i; c--)
            {
               newSequence.Add(solutionNodes.ElementAt(c));
            }

            // 3. Prendi gli elementi da solutionNodes[s+1] all'ultimo e aggiungili
            // nell'ordine di processing a newSequence.
            for (int c = s + 1; c < solutionNodes.Count(); c++)
            {
               newSequence.Add(solutionNodes.ElementAt(c));
            }
         }
         return newSequence;
      }

      private ToSolution GetBest(IEnumerable<ToSolution> solutions) =>
         solutions.MaxBy(solution => solution.Cost);

      private void LockMove(Tuple<int, int> reverseMove)
      {
         RouteWorker edgeMove = Solver.CityMapGraph.GetEdge(reverseMove.Item1, reverseMove.Item2);
         if (edgeMove is null)
         {
            return;
         }

         _executedMoves.Add(edgeMove);
      }

      private string GetMoveDescription(in ToSolution startSolution, in ToSolution newSolution)
      {
         int removedEdgePointFromId = newSolution.Move.Item1;
         int removedEdgePointToId = newSolution.Move.Item2;

         // Prendo giù il RouteWorker corrispondente all'arco che ho cancellato 
         // dalla soluzione di partenza.
         RouteWorker removedEdge = startSolution.SolutionGraph.GetEdge(removedEdgePointFromId, removedEdgePointToId);
         if (removedEdge is null)
         {
            return string.Empty;
         }

         return MessagesRepository.GetMessage(
            MessageCode.LinKernighanMoveDetails,
            newSolution.Id,
            $"({removedEdge.Entity.PointFrom.Id}, {removedEdge.Entity.PointTo.Id})");
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         AcceptImprovementsOnly = false;
         MaxSteps = Parameters[ParameterCodes.MaxIterations];

         Console.ForegroundColor = ConsoleColor.Cyan;
         SendMessage(MessageCode.LinKernighanStart);
         Console.ForegroundColor = ConsoleColor.Gray;

         SendMessage(MessageCode.LinKernighanStartSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);

         _solutionsHistory = new Collection<ToSolution>();
         _executedMoves = new Collection<RouteWorker>();

         //CurrentBestStepSolution = CurrentBestSolution;
         _solutionsHistory.Add(CurrentBestSolution);
      }

      protected override async Task PerformStep()
      {
         Console.ForegroundColor = ConsoleColor.Cyan;
         SendMessage(MessageCode.LinKernighanHStepIncreased, CurrentStep, MaxSteps);
         Console.ForegroundColor = ConsoleColor.Gray;

         ToSolution startStepSolution = _solutionsHistory.Last();
         IEnumerable<InterestPointWorker> solutionNodes = startStepSolution.SolutionGraph.Nodes;
         
         ICollection<ToSolution> stepSolutions = new Collection<ToSolution>();
         for (int j = 0, i = j + 1; j < solutionNodes.Count(); j++, i++)
         {
            if (j == solutionNodes.Count() - 1)
            {
               i = 0;
            }

            // Rimuovo l'arco (j,i).
            InterestPointWorker jNode = solutionNodes.ElementAt(j);
            InterestPointWorker iNode = solutionNodes.ElementAt(i);
            CityMapGraph workingGraph = startStepSolution.SolutionGraph.DeepCopy();
            workingGraph.RemoveEdge(jNode.Entity.Id, iNode.Entity.Id);

            InterestPointWorker sNode = GetClosestSNeighbor(workingGraph, jNode.Entity.Id, iNode.Entity.Id);
            if (sNode is null)
            {
               SendMessage(MessageCode.LinKernighanNoSNodeSelected);
               continue;
            }

            RouteWorker forbiddenMove = _executedMoves.FirstOrDefault(move => move.Entity.PointFrom.Id == jNode.Entity.Id &&
                              move.Entity.PointTo.Id == sNode.Entity.Id);
                                    
            if (forbiddenMove != null)
            {
               SendMessage(MessageCode.LinKernighanBlockedMove, $"(" + forbiddenMove.Entity.PointFrom.Id + "," + forbiddenMove.Entity.PointTo.Id + ")");
               continue;
            }

            int sIndex = solutionNodes.ToList().FindIndex(node => node.Entity.Id == sNode.Entity.Id);
            IEnumerable<InterestPointWorker> newNodeSequence = GenerateSequence(solutionNodes, i, sIndex);
            CityMapGraph newSolutionGraph = BuildSolutionGraph(newNodeSequence);
            Tuple<int, int> newSolutionMove = Tuple.Create(jNode.Entity.Id, iNode.Entity.Id);            
            ToSolution newSolution = new ToSolution()
            {
               SolutionGraph = newSolutionGraph,
               Move = newSolutionMove
            };
            newSolution.Description = GetMoveDescription(startStepSolution, newSolution);
            stepSolutions.Add(newSolution);            
         }

         // Se non ho soluzioni nel mio insieme da validare, ritorno la soluzione corrente con
         // un messaggio.
         if (!stepSolutions.Any())
         {
            return;
         }

         // Valido le soluzioni
         foreach (var solution in stepSolutions)
         {
            SendMessage(solution.Description);
            Solver.EnqueueSolution(solution);
            await Task.Delay(Utils.ValidationDelay).ConfigureAwait(false);

            if (Solver.IsMonitoringEnabled)
            {
               Provider.NotifyObservers(solution);
            }
         }

         await Task.WhenAll(Solver.AlgorithmTasks.Values);

         // Seleziono la migliore soluzione della collezione solutions.
         ToSolution newBestStepSolution = GetBest(stepSolutions);
         _solutionsHistory.Add(newBestStepSolution);

         // Blocco la mossa della best appena selezionata.
         LockMove(newBestStepSolution.Move);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();

         ToSolution bestSolution = CurrentBestSolution;
         SendMessage(ToSolution.SolutionCollectionToString(_solutionsHistory));

         _solutionsHistory.ToList().ForEach(solution =>
         {
            bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, bestSolution.Cost);
            if (isBetterThanCurrentBestSolution)
            {
               bestSolution = solution;
            }
         });
         
         if (CurrentBestSolution.Cost < bestSolution.Cost)
         {
            Console.ForegroundColor = ConsoleColor.Green;
            SendMessage(MessageCode.LinKernighanBestFound, bestSolution.Cost);
            Console.ForegroundColor = ConsoleColor.Gray;
         }
         else
         {
            Console.ForegroundColor = ConsoleColor.Yellow;
            SendMessage(MessageCode.LinKernighanInvariateSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);
            Console.ForegroundColor = ConsoleColor.Gray;
         }

         Solver.BestSolution = bestSolution;
      }

      internal override void OnTerminated()
      {
         SendMessage(MessageCode.LinKernighanStop);
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return CurrentStep > MaxSteps || base.StopConditions();
      } 
      #endregion
   }
}