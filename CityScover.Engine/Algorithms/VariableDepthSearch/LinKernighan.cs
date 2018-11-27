﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 27/11/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
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
      private CityMapGraph _cityMap;
      private CityMapGraph _currentSolutionGraph;
      private ICollection<RouteWorker> _executedMoves;
      private InterestPointWorker _startPoi;
      private InterestPointWorker _endPoi;
      private ICollection<ToSolution> _solutionsHistory;
      #endregion

      #region Constructors
      internal LinKernighan()
         : this(null)
      {         
      }

      internal LinKernighan(AlgorithmTracker provider)
         : base(provider)
      {
         Type = AlgorithmType.LinKernighan;
      }
      #endregion

      #region Internal properties
      internal ToSolution CurrentBestSolution { get; set; }
      internal int MaxSteps { get; set; }
      #endregion

      #region Private methods
      private IEnumerable<InterestPointWorker> GetClosestSNeighbors()
      {
         int predEndPoiNodeId = _currentSolutionGraph.GetPredecessorNodes(_endPoi.Entity.Id).FirstOrDefault();
         IEnumerable<InterestPointWorker> sCandidates = _currentSolutionGraph.TourPoints
            .Where(node => node.Entity.Id != predEndPoiNodeId && 
                           node.Entity.Id != _endPoi.Entity.Id)
            .OrderByDescending(node => node.Entity.Score.Value);
          
         //return from neighbor in GetClosestNeighbors()
         //       from edge in _currentSolutionGraph.GetEdges(neighbor.Entity.Id)
         //       where edge.Entity.PointTo.Id != _endPOI.Entity.Id
         //       && _currentSolutionGraph.ContainsNode(neighbor.Entity.Id)
         //       select neighbor;
         return sCandidates;
      }

      //private IEnumerable<InterestPointWorker> GetClosestNeighbors()
      //{
      //   int bestScore = default;
      //   ICollection<InterestPointWorker> potentialCandidates = new Collection<InterestPointWorker>();

      //   var adjPOIIds = _cityMap.GetAdjacentNodes(_endPOI.Entity.Id);
      //   adjPOIIds.ToList().ForEach(adjPOIId =>
      //   {
      //      InterestPointWorker candidateNode = default;
      //      var node = _cityMap[adjPOIId];
      //      if (node.Entity.Id == _startPOI.Entity.Id)
      //      {
      //         return;
      //      }

      //      var deltaScore = Math.Abs(node.Entity.Score.Value - _endPOI.Entity.Score.Value);
      //      if (deltaScore > bestScore)
      //      {
      //         bestScore = deltaScore;
      //         candidateNode = node;
      //      }
      //      else if (deltaScore == bestScore)
      //      {
      //         CityMapGraph.SetRandomCandidateId(candidateNode, node, out int pointId);
      //         candidateNode = _cityMap[pointId];
      //      }

      //      if (candidateNode != null)
      //      {
      //         potentialCandidates.Add(candidateNode);
      //      }
      //   });

      //   return potentialCandidates.OrderByDescending(node => node.Entity.Score.Value);
      //}

      private void SwapNodes(int stopSwappingNodeId)
      {
         int currentNodeId = _startPoi.Entity.Id;
         while (currentNodeId != stopSwappingNodeId)
         {
            var currNodeAdjNode = _currentSolutionGraph.GetAdjacentNodes(currentNodeId).FirstOrDefault();
            if (currNodeAdjNode == 0)
            {
               throw new InvalidOperationException();
            }

            _currentSolutionGraph.RemoveEdge(currentNodeId, currNodeAdjNode);
            _currentSolutionGraph.AddRouteFromGraph(_cityMap, currNodeAdjNode, currentNodeId);
            currentNodeId = currNodeAdjNode;
         }
      }

      // Funzione che costruisce un nuovo ciclo hamiltoniano.
      // La funzione restituisce l'ID dell'altro nodo dell'arco che vado a togliere, poichè
      // è il punto di partenza per la chiusura del ciclo.
      private int BuildHamiltonianPath(int sPoiId)
      {
         // Rimuovo l'unico arco di s. l'arco (j,s) per via della struttura Meriottesca è posseduto da j non da s.
         RouteWorker sEdge = _currentSolutionGraph.GetEdges(sPoiId).FirstOrDefault();
         if (sEdge is null)
         {
            throw new InvalidOperationException();
         }

         int sEdgePointToId = sEdge.Entity.PointTo.Id;
         _currentSolutionGraph.RemoveEdge(sPoiId, sEdgePointToId);
         return sEdgePointToId;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         AcceptImprovementsOnly = false;
         Console.ForegroundColor = ConsoleColor.Cyan;
         SendMessage(MessageCode.LKStarting);
         Console.ForegroundColor = ConsoleColor.Gray;

         SendMessage(MessageCode.LKStartSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);

         _solutionsHistory = new Collection<ToSolution>();
         _cityMap = Solver.CityMapGraph.DeepCopy();
         _executedMoves = new Collection<RouteWorker>();

         _currentSolutionGraph = CurrentBestSolution.SolutionGraph.DeepCopy();

         _startPoi = CurrentBestSolution.SolutionGraph.GetStartPoint();
         _endPoi = CurrentBestSolution.SolutionGraph.GetEndPoint();

         if (_startPoi is null || _endPoi is null)
         {
            throw new NullReferenceException();
         }
      }

      protected override async Task PerformStep()
      {
         Console.ForegroundColor = ConsoleColor.Cyan;
         SendMessage(MessageCode.LKHStepIncreased, CurrentStep, MaxSteps);
         Console.ForegroundColor = ConsoleColor.Gray;
         
         InterestPointWorker sNode = default;

         // Tolgo l'arco (j,i).
         SendMessage(_currentSolutionGraph.ToString());
         _currentSolutionGraph.RemoveEdge(_endPoi.Entity.Id, _startPoi.Entity.Id);

         var sNodesCandidates = GetClosestSNeighbors();
         foreach (var sNodeCandidate in sNodesCandidates)
         {
            RouteWorker fromEndNodeToSNodeEdge = _cityMap.GetEdge(_endPoi.Entity.Id, sNodeCandidate.Entity.Id);
            if (!_executedMoves.Contains(fromEndNodeToSNodeEdge))
            {
               sNode = sNodeCandidate;
               // Build Steam And Cycle.
               _currentSolutionGraph.AddRouteFromGraph(_cityMap, _endPoi.Entity.Id, sNodeCandidate.Entity.Id);
               _executedMoves.Add(fromEndNodeToSNodeEdge);
               break;
            }
            SendMessage(MessageCode.LKBlockedMove, $"(" + fromEndNodeToSNodeEdge.Entity.PointFrom.Id + "," + fromEndNodeToSNodeEdge.Entity.PointTo.Id + ")");
         }

         if (sNode is null)
         {
            SendMessage(MessageCode.LKNoSNodeSelected);
            ForceStop = true;
            return;
         }
         //SendMessage("S node is " + sNode.Entity.Id);

         int junctionNodeId = BuildHamiltonianPath(sNode.Entity.Id);

         SwapNodes(sNode.Entity.Id);

         // Poi ricreo il ciclo.         
         _currentSolutionGraph.AddRouteFromGraph(_cityMap, _startPoi.Entity.Id, junctionNodeId);
         
         ToSolution newSolution = new ToSolution()
         {
            SolutionGraph = _currentSolutionGraph.DeepCopy()
         };
         _solutionsHistory.Add(newSolution);
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);

         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(newSolution);
         }

         _endPoi = _currentSolutionGraph.GetEndPoint();
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
            SendMessage(MessageCode.LKBestFound, bestSolution.Cost);
         }
         else
         {
            SendMessage(MessageCode.LKInvariateSolution, CurrentBestSolution.Id, CurrentBestSolution.Cost);
         }

         Solver.BestSolution = bestSolution;
      }

      internal override void OnTerminated()
      {
         _cityMap = null;
         SendMessage(MessageCode.LKFinish);
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return CurrentStep > MaxSteps || base.StopConditions();
      } 
      #endregion
   }
}