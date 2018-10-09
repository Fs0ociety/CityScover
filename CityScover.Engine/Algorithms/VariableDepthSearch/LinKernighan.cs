//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/10/2018
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
      private CityMapGraph _cityMapClone;
      private CityMapGraph _currentSolutionGraph;
      private TOSolution _bestSolution;
      private TOSolution _currentSolution;
      private ICollection<RouteWorker> _executedMoves;
      private InterestPointWorker _startPOI;
      private InterestPointWorker _endPOI;
      private byte _executedSteps;

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
      internal byte MaxSteps { get; private set; }
      #endregion

      #region Private methods
      private IEnumerable<InterestPointWorker> GetClosestSNeighbors(InterestPointWorker endPOI, CityMapGraph currentSolutionGraph)
      {
         //IEnumerable<InterestPointWorker> endPOINeighbors = GetClosestNeighbors(endPOI);
         //IEnumerable<int> endPOIAdjNodes = _cityMapClone.GetAdjacentNodes(endPOI.Entity.Id);
         //ICollection<InterestPointWorker> sCandidates = new Collection<InterestPointWorker>();

         //foreach (var neighbor in endPOINeighbors)
         //{
         //   foreach (var adjNode in endPOIAdjNodes)
         //   {
         //      if (_currentSolutionGraph.ContainsNode(adjNode) && neighbor.Entity.Id != adjNode)
         //      {
         //         sCandidates.Add(neighbor);
         //      }
         //   }
         //}

         var sCandidates = from neighbor in GetClosestNeighbors(endPOI)
                           from adjNode in _cityMapClone.GetAdjacentNodes(endPOI.Entity.Id)
                           where neighbor.Entity.Id != adjNode
                           && _currentSolutionGraph.ContainsNode(adjNode)
                           select neighbor;

         return sCandidates;
      }

      private IEnumerable<InterestPointWorker> GetClosestNeighbors(InterestPointWorker endPOI)
      {
         int bestScore = default;
         IList<InterestPointWorker> potentialCandidates = new List<InterestPointWorker>();

         var adjPOIIds = _cityMapClone.GetAdjacentNodes(endPOI.Entity.Id);
         adjPOIIds.ToList().ForEach(adjPOIId => SetBestCandidate(adjPOIId));

         // Caso particolare (gestito solo per irrobustire il codice): se ho 2 nodi del grafo, e
         // il secondo è già stato visitato, io ritorno collezione vuota come potentialCandidates.

         // First local function: SetBestCandidate
         void SetBestCandidate(int nodeKey)
         {

            InterestPointWorker candidateNode = default;
            var node = _cityMapClone[nodeKey];
            if (node.IsVisited)
            {
               return;
            }

            var deltaScore = Math.Abs(node.Entity.Score.Value -
               endPOI.Entity.Score.Value);

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
               if (candidateNode == null)
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
            
            potentialCandidates.Insert(0, candidateNode);
         }

         return potentialCandidates;
      }

      private (CityMapGraph steamSet, CityMapGraph cycleSet) BuildSteamAndCycle(InterestPointWorker sNode)
      {
         CityMapGraph steamSet = new CityMapGraph();
         CityMapGraph cycleSet = new CityMapGraph();

         cycleSet.AddRouteFromGraph(_cityMapClone, _endPOI.Entity.Id, sNode.Entity.Id);
         foreach (var edge in _currentSolutionGraph.Edges)
         {
            
         }
         return (steamSet, cycleSet);
      }

      private void BuildHamiltonianPath((CityMapGraph steamSet, CityMapGraph cycleSet) p, CityMapGraph currentSolutionGraph)
      {
         throw new NotImplementedException();
      }

      private void ConnectHamiltonianPath(CityMapGraph currentSolutionGraph)
      {
         throw new NotImplementedException();
      } 
      #endregion

      #region Overrides
      internal override void OnError()
      {
         throw new System.NotImplementedException();
      }

      internal override void OnInitializing()
      {
         base.OnInitializing();

         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         _executedMoves = new Collection<RouteWorker>();
         MaxSteps = 2;
         // TODO: la prende dal solver?
         _bestSolution = Solver.BestSolution;
         _currentSolutionGraph = _bestSolution.SolutionGraph.DeepCopy();

         var startPOIId = Solver.WorkingConfiguration.StartPOIId;
         _startPOI = _cityMapClone.Nodes
            .Where(x => x.Entity.Id == startPOIId)
            .FirstOrDefault();
         
         _endPOI = _cityMapClone.Nodes.Where(
            node => node.Entity.Id.Equals(_cityMapClone.Edges.Where(
               edge => edge.Entity.PointTo.Id == Solver.WorkingConfiguration.StartPOIId).Select(edge => edge.Entity.PointFrom.Id).FirstOrDefault())).FirstOrDefault();

         if (_startPOI == null || _endPOI == null)
         {
            throw new NullReferenceException();
         }

         //InterestPointWorker GetEndPOI()
         //{
         //   var result = (from node in _cityMapClone.Nodes
         //                 where node.Entity.Id == (from edge in _cityMapClone.Edges
         //                                          where edge.Entity.PointTo.Id == Solver.WorkingConfiguration.StartPOIId
         //                                          select edge.Entity.PointFrom.Id).FirstOrDefault()
         //                 select node).FirstOrDefault();
         //   return result;
         //}
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
         _cityMapClone = null;
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();

         // Notifica gli observers.
         // TODO

         bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(_currentSolution.Cost, _bestSolution.Cost);
         if (isBetterThanCurrentBestSolution)
         {
            _bestSolution = _currentSolution;            
         }
      }

      internal override async Task PerformStep()
      {
         InterestPointWorker sNode = default;
         var sNodesCandidates = GetClosestSNeighbors(_endPOI, _currentSolutionGraph);
         foreach (var node in sNodesCandidates)
         {
            RouteWorker fromEndNodeToSNodeEdge = _cityMapClone.GetEdge(_endPOI.Entity.Id, node.Entity.Id);
            if (!_executedMoves.Contains(fromEndNodeToSNodeEdge))
            {
               sNode = node;
               _executedMoves.Add(fromEndNodeToSNodeEdge);
               break;
            }
         }

         var (steamSet, cycleSet) = BuildSteamAndCycle(sNode);
         BuildHamiltonianPath((steamSet, cycleSet), _currentSolutionGraph);
         ConnectHamiltonianPath(_currentSolutionGraph);

         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _currentSolutionGraph.DeepCopy()
         };
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(500).ConfigureAwait(continueOnCapturedContext: false);

         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(newSolution);
         }
         _executedSteps++;
      }

      internal override bool StopConditions()
      {
         return _executedSteps == MaxSteps;
      } 
      #endregion
   }
}
