//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 21/12/2018
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
      //private CityMapGraph _cityMap;
      //private CityMapGraph _currentSolutionGraph;
      private ICollection<RouteWorker> _executedMoves;
      //private InterestPointWorker _startPoi;
      //private InterestPointWorker _endPoi;
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
      //private IEnumerable<InterestPointWorker> GetClosestSNeighbors(int iNodeId, int jNodeId)
      //{
      //   int predEndPoiNodeId = _currentSolutionGraph.GetPredecessorNodes(jNodeId).FirstOrDefault();
      //   IEnumerable<InterestPointWorker> sCandidates = _currentSolutionGraph.TourPoints
      //      .Where(node => node.Entity.Id != predEndPoiNodeId && 
      //                     node.Entity.Id != jNodeId)
      //      .OrderByDescending(node => node.Entity.Score.Value);
      //   return sCandidates;
      //}

      private InterestPointWorker GetSNode(int jNodeId, int iNodeId)
      {
         int predEndPoiNodeId = CurrentBestStepSolution.SolutionGraph.GetPredecessorNodes(jNodeId).FirstOrDefault();
         IEnumerable<InterestPointWorker> sCandidates = CurrentBestStepSolution.SolutionGraph.TourPoints
            .Where(node => node.Entity.Id != predEndPoiNodeId &&
                           node.Entity.Id != jNodeId);

         if (!sCandidates.Any())
         {
            return null;
         }

         return sCandidates.MaxBy(node => node.Entity.Score.Value);
      }

      //private void SwapNodes(int stopSwappingNodeId)
      //{
      //   int currentNodeId = _startPoi.Entity.Id;
      //   while (currentNodeId != stopSwappingNodeId)
      //   {
      //      var currNodeAdjNode = _currentSolutionGraph.GetAdjacentNodes(currentNodeId).FirstOrDefault();
      //      if (currNodeAdjNode == 0)
      //      {
      //         throw new InvalidOperationException();
      //      }

      //      _currentSolutionGraph.RemoveEdge(currentNodeId, currNodeAdjNode);
      //      _currentSolutionGraph.AddRouteFromGraph(_cityMap, currNodeAdjNode, currentNodeId);
      //      currentNodeId = currNodeAdjNode;
      //   }
      //}

      // Funzione che costruisce un nuovo ciclo hamiltoniano.
      // La funzione restituisce l'ID dell'altro nodo dell'arco che vado a togliere, poichè
      // è il punto di partenza per la chiusura del ciclo.
      //private int BuildHamiltonianPath(int sPoiId)
      //{
      //   // Rimuovo l'unico arco di s. l'arco (j,s) per via della struttura Meriottesca è posseduto da j non da s.
      //   RouteWorker sEdge = _currentSolutionGraph.GetEdges(sPoiId).FirstOrDefault();
      //   if (sEdge is null)
      //   {
      //      throw new InvalidOperationException();
      //   }

      //   int sEdgePointToId = sEdge.Entity.PointTo.Id;
      //   _currentSolutionGraph.RemoveEdge(sPoiId, sEdgePointToId);
      //   return sEdgePointToId;
      //}

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
         //_cityMap = Solver.CityMapGraph.DeepCopy();
         _executedMoves = new Collection<RouteWorker>();

         CurrentBestStepSolution = CurrentBestSolution;

         //_currentSolutionGraph = CurrentBestSolution.SolutionGraph.DeepCopy();
         //_startPoi = CurrentBestSolution.SolutionGraph.GetStartPoint();
         //_endPoi = CurrentBestSolution.SolutionGraph.GetEndPoint();

         //if (_startPoi is null || _endPoi is null)
         //{
         //   throw new NullReferenceException();
         //}
      }

      protected override async Task PerformStep()
      {
         Console.ForegroundColor = ConsoleColor.Cyan;
         SendMessage(MessageCode.LinKernighanHStepIncreased, CurrentStep, MaxSteps);
         Console.ForegroundColor = ConsoleColor.Gray;

         // Metto il primo nodo della sequenza in fondo.
         IList<InterestPointWorker> solutionNodes = CurrentBestStepSolution.SolutionGraph.Nodes.ToList();
         InterestPointWorker firstNode = solutionNodes.FirstOrDefault();
         solutionNodes.Remove(firstNode);
         solutionNodes.Append(firstNode);

         ICollection<ToSolution> stepSolutions = new Collection<ToSolution>();
         for (int j = 0; i < solutionNodes.Count() - 1; j++)
         {
            for (int i = j + 1; i < solutionNodes.Count(); i++)
            {
               // Rimuovo l'arco (j,i).
               CurrentBestStepSolution.SolutionGraph.RemoveEdge(j, i);

               InterestPointWorker sNode = GetSNode(j, i);
               if (sNode is null)
               {
                  SendMessage(MessageCode.LinKernighanNoSNodeSelected);
                  ForceStop = true;
                  return;
               }

               IEnumerable<InterestPointWorker> newNodeSequence = GenerateSequence(solutionNodes, i, sNode.Entity.Id);
               CityMapGraph newSolutionGraph = BuildSolutionGraph(newNodeSequence);
               //Tuple<int, int> move = GetMove(solution, solutionNodes.ElementAt(i).Entity.Id, solutionNodes.ElementAt(k).Entity.Id);
               //if (move is null)
               //{
               //   continue;
               //}
               ToSolution newSolution = new ToSolution()
               {
                  SolutionGraph = newSolutionGraph
                  //Move = move
               };
               //newSolution.Description = GetMoveDescription(solution, newSolution);
               stepSolutions.Add(newSolution);
            }
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
         ToSolution bestStepSolution = GetBest(stepSolutions);

         //InterestPointWorker sNode = default;

         //// Tolgo l'arco (j,i).
         //SendMessage(_currentSolutionGraph.ToString());
         //_currentSolutionGraph.RemoveEdge(_endPoi.Entity.Id, _startPoi.Entity.Id);

         //var sNodesCandidates = GetClosestSNeighbors();
         //foreach (var sNodeCandidate in sNodesCandidates)
         //{
         //   RouteWorker fromEndNodeToSNodeEdge = _cityMap.GetEdge(_endPoi.Entity.Id, sNodeCandidate.Entity.Id);
         //   if (!_executedMoves.Contains(fromEndNodeToSNodeEdge))
         //   {
         //      sNode = sNodeCandidate;
         //      // Build Steam And Cycle.
         //      _currentSolutionGraph.AddRouteFromGraph(_cityMap, _endPoi.Entity.Id, sNodeCandidate.Entity.Id);
         //      _executedMoves.Add(fromEndNodeToSNodeEdge);
         //      break;
         //   }
         //   SendMessage(MessageCode.LinKernighanBlockedMove, $"(" + fromEndNodeToSNodeEdge.Entity.PointFrom.Id + "," + fromEndNodeToSNodeEdge.Entity.PointTo.Id + ")");
         //}

         //if (sNode is null)
         //{
         //   SendMessage(MessageCode.LinKernighanNoSNodeSelected);
         //   ForceStop = true;
         //   return;
         //}

         //int junctionNodeId = BuildHamiltonianPath(sNode.Entity.Id);

         //SwapNodes(sNode.Entity.Id);

         //// Poi ricreo il ciclo.         
         //_currentSolutionGraph.AddRouteFromGraph(_cityMap, _startPoi.Entity.Id, junctionNodeId);
         
         //ToSolution newSolution = new ToSolution()
         //{
         //   SolutionGraph = _currentSolutionGraph.DeepCopy()
         //};
         //_solutionsHistory.Add(newSolution);
         //Solver.EnqueueSolution(newSolution);
         //await Task.Delay(Utils.ValidationDelay).ConfigureAwait(continueOnCapturedContext: false);

         //if (Solver.IsMonitoringEnabled)
         //{
         //   Provider.NotifyObservers(newSolution);
         //}

         //_endPoi = _currentSolutionGraph.GetEndPoint();
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