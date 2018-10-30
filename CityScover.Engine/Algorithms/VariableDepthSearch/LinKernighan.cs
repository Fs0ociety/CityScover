//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 27/10/2018
//

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
      private CityMapGraph _cityMapClone;
      private CityMapGraph _currentSolutionGraph;
      private TOSolution _currentSolution;
      private ICollection<RouteWorker> _executedMoves;
      private InterestPointWorker _startPOI;
      private InterestPointWorker _endPOI;
      private byte _executedSteps;
      #endregion

      #region Constructors
      internal LinKernighan()
         : this(null)
      {         
      }

      internal LinKernighan(AlgorithmTracker tracker)
         : base(tracker)
      {
      }
      #endregion

      #region Internal properties
      internal TOSolution CurrentBestSolution { get; set; }
      internal byte MaxSteps { get; set; }
      #endregion

      #region Private methods
      private IEnumerable<InterestPointWorker> GetClosestSNeighbors()
      {
         return from neighbor in GetClosestNeighbors()
                from edge in _currentSolutionGraph.GetEdges(neighbor.Entity.Id)
                where edge.Entity.PointTo.Id != _endPOI.Entity.Id
                && _currentSolutionGraph.ContainsNode(neighbor.Entity.Id)
                select neighbor;
      }

      private IEnumerable<InterestPointWorker> GetClosestNeighbors()
      {
         int bestScore = default;
         ICollection<InterestPointWorker> potentialCandidates = new Collection<InterestPointWorker>();

         var adjPOIIds = _cityMapClone.GetAdjacentNodes(_endPOI.Entity.Id).Where(nodeId => nodeId != _startPOI.Entity.Id);
         adjPOIIds.ToList().ForEach(adjPOIId => SetBestCandidate(adjPOIId));

         // Caso particolare (gestito solo per irrobustire il codice): se ho 2 nodi del grafo, e
         // il secondo è già stato visitato, io ritorno collezione vuota come potentialCandidates.

         // First local function: SetBestCandidate
         void SetBestCandidate(int nodeKey)
         {

            InterestPointWorker candidateNode = default;
            var node = _cityMapClone[nodeKey];
            //if (node.IsVisited)
            //{
            //   return;
            //}

            var deltaScore = Math.Abs(node.Entity.Score.Value -
               _endPOI.Entity.Score.Value);

            if (deltaScore > bestScore)
            {
               bestScore = deltaScore;
               candidateNode = node;
            }
            else if (deltaScore == bestScore)
            {
               SetRandomCandidateId(out int pointId);
               candidateNode = _cityMapClone[pointId];
            }

            // Second local function: SetRandomCandidateId
            void SetRandomCandidateId(out int id)
            {
               if (candidateNode is null)
               {
                  id = node.Entity.Id;
               }
               else
               {
                  id = (new Random().Next(2) != 0)
                     ? candidateNode.Entity.Id
                     : node.Entity.Id;
               }
            }

            if (candidateNode != null)
            {
               potentialCandidates.Add(candidateNode);
            }
         }

         return potentialCandidates.OrderByDescending(node => node.Entity.Score.Value);
      }

      //private (CityMapGraph steamSet, CityMapGraph cycleSet) BuildSteamAndCycle(int sNodeId)
      //{
      //   CityMapGraph steamSet = new CityMapGraph();
      //   CityMapGraph cycleSet = new CityMapGraph();

      //   cycleSet.AddNodeFromGraph(_currentSolutionGraph, _endPOI.Entity.Id);
      //   cycleSet.AddNodeFromGraph(_currentSolutionGraph, sNodeId);
      //   cycleSet.AddRouteFromGraph(_cityMapClone, _endPOI.Entity.Id, sNodeId);
      //   _currentSolutionGraph.Edges.ToList().ForEach(
      //      edge =>
      //      {
      //         if (edge.Entity.PointFrom.Id == _startPOI.Entity.Id || edge.Entity.PointTo.Id == _startPOI.Entity.Id)
      //         {
      //            steamSet.AddNodeFromGraph(_currentSolutionGraph, edge.Entity.PointFrom.Id);
      //            steamSet.AddNodeFromGraph(_currentSolutionGraph, edge.Entity.PointTo.Id);
      //            steamSet.AddRouteFromGraph(_currentSolutionGraph, edge.Entity.PointFrom.Id, edge.Entity.PointTo.Id);
      //         }
      //         else
      //         {
      //            if (edge.Entity.PointFrom.Id != sNodeId)
      //            {
      //               cycleSet.AddNodeFromGraph(_currentSolutionGraph, edge.Entity.PointFrom.Id);
      //               cycleSet.AddNodeFromGraph(_currentSolutionGraph, edge.Entity.PointTo.Id);
      //               cycleSet.AddRouteFromGraph(_currentSolutionGraph, edge.Entity.PointFrom.Id, edge.Entity.PointTo.Id);
      //            }
      //         }
      //      });
      //   return (steamSet, cycleSet);
      //}

      //private void BuildHamiltonianPath((CityMapGraph steamSet, CityMapGraph cycleSet) steamAndCycle, int sNodeId)
      //{
      //   steamAndCycle.steamSet.AddGraph(steamAndCycle.cycleSet, sNodeId);
      //   _currentSolutionGraph = steamAndCycle.steamSet;

      //   // Connetto il ciclo nell'unico modo possibile.
      //   RouteWorker result = (from edge in _currentSolutionGraph.Edges
      //                         where edge.Entity.PointFrom.Id != _startPOI.Entity.Id
      //                         && _currentSolutionGraph.GetNodeGrade(edge.Entity.PointFrom.Id) == 2
      //                         select edge).FirstOrDefault();
      //   if (result == null)
      //   {
      //      throw new InvalidOperationException();
      //   }

      //   _currentSolutionGraph.AddEdge(result.Entity.PointFrom.Id, result.Entity.PointTo.Id, result);
      //}

      private void SwapNodes(int stopSwappingNodeId)
      {
         int currentNodeId = _startPOI.Entity.Id;
         while (currentNodeId != stopSwappingNodeId)
         {
            var currNodeAdjNode = _currentSolutionGraph.GetAdjacentNodes(currentNodeId).FirstOrDefault();
            if (currNodeAdjNode == 0)
            {
               throw new InvalidOperationException();
            }

            _currentSolutionGraph.RemoveEdge(currentNodeId, currNodeAdjNode);
            _currentSolutionGraph.AddRouteFromGraph(_cityMapClone, currNodeAdjNode, currentNodeId);
            currentNodeId = currNodeAdjNode;
         }
      }
      #endregion

      #region Overrides
      internal override void OnError(Exception exception)
      {
         // Da gestire timeSpent (probabilmente con metodo che somma i tempi di tutti i nodi).
         Result resultError =
            new Result(CurrentBestSolution, CurrentAlgorithm, null, Result.Validity.Invalid);
         resultError.ResultFamily = AlgorithmFamily.Improvement;
         Solver.Results.Add(resultError);
         base.OnError(exception);
      }

      internal override void OnInitializing()
      {
         base.OnInitializing();

         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         _executedMoves = new Collection<RouteWorker>();

         _currentSolution = new TOSolution
         {
            SolutionGraph = CurrentBestSolution.SolutionGraph.DeepCopy()
         };

         _currentSolutionGraph = _currentSolution.SolutionGraph;

         _startPOI = _currentSolutionGraph.GetStartPoint();
         _endPOI = _currentSolutionGraph.GetEndPoint();

         if (_startPOI is null || _endPOI is null)
         {
            throw new NullReferenceException();
         }

         // Tolgo l'arco (j,i).
         _currentSolutionGraph.RemoveEdge(_endPOI.Entity.Id, _startPOI.Entity.Id);
      }

      internal override void OnTerminated()
      {
         // Da gestire timeSpent (probabilmente con metodo che somma i tempi di tutti i nodi).
         Result validResult =
            new Result(CurrentBestSolution, CurrentAlgorithm, null, Result.Validity.Valid);
         validResult.ResultFamily = AlgorithmFamily.Improvement;
         Solver.Results.Add(validResult);
         _cityMapClone = null;
         base.OnTerminated();
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();

         bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(_currentSolution.Cost, CurrentBestSolution.Cost);
         if (isBetterThanCurrentBestSolution)
         {
            CurrentBestSolution = _currentSolution;            
         }
      }

      internal override async Task PerformStep()
      {
         InterestPointWorker sNode = default;
         var sNodesCandidates = GetClosestSNeighbors();
         foreach (var sNodeCandidate in sNodesCandidates)
         {
            RouteWorker fromEndNodeToSNodeEdge = _cityMapClone.GetEdge(_endPOI.Entity.Id, sNodeCandidate.Entity.Id);
            if (!_executedMoves.Contains(fromEndNodeToSNodeEdge))
            {
               sNode = sNodeCandidate;
               _executedMoves.Add(fromEndNodeToSNodeEdge);
               break;
            }
         }

         if (sNode is null)
         {
            return;
         }

         int sNodeId = sNode.Entity.Id;

         // Build Steam And Cycle.
         _currentSolutionGraph.AddRouteFromGraph(_cityMapClone, _endPOI.Entity.Id, sNodeId);
         
         int junctionNodeId = BuildHamiltonianPath(sNodeId);

         // Ricreo il ciclo nell'unico modo possibile dal nuovo cammino hamiltoniano.
         // Un cazzo.. Il taccone della Nonato ancoraaa??
         SwapNodes(_startPOI.Entity.Id);

         // Poi ricreo il ciclo.
         _currentSolutionGraph.AddRouteFromGraph(_cityMapClone, _startPOI.Entity.Id, junctionNodeId
            );
         
         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _currentSolutionGraph.DeepCopy()
         };
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);

         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(newSolution);
         }
         _executedSteps++;
         _currentSolution = newSolution;

         // Local function per costruire un nuovo ciclo hamiltoniano.
         // La funzione restituisce l'ID dell'altro nodo dell'arco che vado a togliere, poichè
         // è il punto di partenza per la chiusura del ciclo.
         int BuildHamiltonianPath(int sPOIId)
         {
            // Rimuovo l'unico arco di s. l'arco (j,s) per via della struttura Meriottesca è posseduto da j non da s.
            RouteWorker sEdge = _currentSolutionGraph.GetEdges(sPOIId).FirstOrDefault();
            if (sEdge is null)
            {
               throw new InvalidOperationException();
            }

            int sEdgePointToId = sEdge.Entity.PointTo.Id;
            _currentSolutionGraph.RemoveEdge(sPOIId, sEdgePointToId);
            return sEdgePointToId;
         }
      }

      internal override bool StopConditions()
      {
         return _executedSteps == MaxSteps;
      } 
      #endregion
   }
}
