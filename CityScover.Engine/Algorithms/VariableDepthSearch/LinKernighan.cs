//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 22/12/2018
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
      internal ToSolution CurrentBestStepSolution { get; set; }
      internal int MaxSteps { get; set; }
      #endregion

      #region Private methods
      private IEnumerable<InterestPointWorker> GetClosestSNeighbors(CityMapGraph workingGraph, int jNodeId, int iNodeId)
      {
         int predEndPoiNodeId = workingGraph.GetPredecessorNodes(jNodeId).FirstOrDefault();
         IEnumerable<InterestPointWorker> sCandidates = workingGraph.Nodes
            .Where(node => node.Entity.Id != predEndPoiNodeId &&
                           node.Entity.Id != iNodeId &&
                           node.Entity.Id != jNodeId)
            .OrderByDescending(node => node.Entity.Score.Value);
         return sCandidates;
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

         CurrentBestStepSolution = CurrentBestSolution;
      }

      protected override async Task PerformStep()
      {
         Console.ForegroundColor = ConsoleColor.Cyan;
         SendMessage(MessageCode.LinKernighanHStepIncreased, CurrentStep, MaxSteps);
         Console.ForegroundColor = ConsoleColor.Gray;

         // Metto il primo nodo della sequenza in fondo.
         ICollection<InterestPointWorker> solutionNodes = CurrentBestStepSolution.SolutionGraph.Nodes.ToList();
         InterestPointWorker firstNode = solutionNodes.FirstOrDefault();
         solutionNodes.Remove(firstNode);
         solutionNodes.Add(firstNode);


         ICollection<ToSolution> stepSolutions = new Collection<ToSolution>();
         for (int j = 0, i = j + 1; j < solutionNodes.Count(); j++, i++)
         {
            if (j == solutionNodes.Count - 1)
            {
               i = 0;
            }

            // Rimuovo l'arco (j,i).
            InterestPointWorker jNode = solutionNodes.ElementAt(j);
            InterestPointWorker iNode = solutionNodes.ElementAt(i);
            CityMapGraph workingGraph = CurrentBestStepSolution.SolutionGraph.DeepCopy();
            workingGraph.RemoveEdge(jNode.Entity.Id, iNode.Entity.Id);

            InterestPointWorker sNode = default;
            var sNodeCandidates = GetClosestSNeighbors(workingGraph, jNode.Entity.Id, iNode.Entity.Id);
            foreach (var sNodeCandidate in sNodeCandidates)
            {
               RouteWorker forbiddenMove = _executedMoves.FirstOrDefault(move => move.Entity.PointFrom.Id == jNode.Entity.Id &&
                                 move.Entity.PointTo.Id == sNodeCandidate.Entity.Id);
                                    
               if (forbiddenMove is null)
               {
                  sNode = sNodeCandidate;
                  break;
               }
               //SendMessage(MessageCode.LinKernighanBlockedMove, $"(" + fromEndNodeToSNodeEdge.Entity.PointFrom.Id + "," + fromEndNodeToSNodeEdge.Entity.PointTo.Id + ")");
            }

            if (sNode is null)
            {
               SendMessage(MessageCode.LinKernighanNoSNodeSelected);
               ForceStop = true;
               return;
            }

            int sIndex = solutionNodes.ToList().FindIndex(node => node.Entity.Id == sNode.Entity.Id);
            IEnumerable<InterestPointWorker> newNodeSequence = GenerateSequence(solutionNodes, i, sIndex);
            CityMapGraph newSolutionGraph = BuildSolutionGraph(newNodeSequence);
            Tuple<int, int> newSolutionMove = Tuple.Create(jNode.Entity.Id, iNode.Entity.Id);
            if (newSolutionMove is null)
            {
               continue;
            }
            ToSolution newSolution = new ToSolution()
            {
               SolutionGraph = newSolutionGraph,
               Move = newSolutionMove
            };
            //newSolution.Description = GetMoveDescription(solution, newSolution);
            stepSolutions.Add(newSolution);            
         }

         // Valido le soluzioni
         foreach (var solution in stepSolutions)
         {
            Solver.EnqueueSolution(solution);
            await Task.Delay(Utils.ValidationDelay).ConfigureAwait(false);

            if (Solver.IsMonitoringEnabled)
            {
               Provider.NotifyObservers(solution);
            }
         }

         await Task.WhenAll(Solver.AlgorithmTasks.Values);

         // Seleziono la migliore soluzione della collezione solutions.
         CurrentBestStepSolution = GetBest(stepSolutions);

         // Blocco la mossa della best appena selezionata.
         LockMove(CurrentBestStepSolution.Move);
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
         
         if (CurrentBestSolution.Cost != bestSolution.Cost)
         {
            SendMessage(MessageCode.LinKernighanBestFound, bestSolution.Cost);
         }
         else
         {
            SendMessage(MessageCode.LinKernighanInvariateSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);
         }

         Solver.BestSolution = bestSolution;
      }

      internal override void OnTerminated()
      {
         //_cityMap = null;
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