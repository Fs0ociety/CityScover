//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 01/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// TODO
   /// </summary>
   internal class NearestNeighborAlgorithm : Algorithm
   {
      private double _averageSpeedWalk;
      private ICollection<TOSolution> _solutions;
      protected CityMapGraph _cityMapClone;
      private CityMapGraph _currentSolutionGraph;
      private InterestPointWorker _startPOI;
      private InterestPointWorker _newStartPOI;
      private DateTime _timeSpent;

      #region Constructors
      internal NearestNeighborAlgorithm()
         : this(null)
      {
      }

      internal NearestNeighborAlgorithm(AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      /// <summary>
      /// This implementation is the classic GetClosestNeighborByScore.
      /// It returns the best candidate node near to point of interest passed as argument.
      /// </summary>
      /// <param name="interestPoint">Point of interest</param>
      /// <returns></returns>
      protected virtual InterestPointWorker GetBestNeighbor(InterestPointWorker interestPoint)
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;

         var adjPOIIds = _cityMapClone.GetAdjacentNodes(interestPoint.Entity.Id);
         adjPOIIds.ToList().ForEach(adjPOIId => SetBestCandidate(adjPOIId));

         // Caso particolare (gestito solo per irrobustire il codice): se ho 2 nodi del grafo, e
         // il secondo è già stato visitato, io ritorno null come candidateNode.

         // First local function: SetBestCandidate
         void SetBestCandidate(int nodeKey)
         {
            var node = _cityMapClone[nodeKey];

            if (node.IsVisited)
            {
               return;
            }

            var deltaScore = Math.Abs(node.Entity.Score.Value -
               interestPoint.Entity.Score.Value);

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
         }

         return candidateNode;
      }
      #endregion

      #region Overrides
      internal override void OnError()
      {
         base.OnError();
         _currentStep = default;
         TOSolution lastProducedSolution = _solutions.Last();
         Result resultError = new Result(lastProducedSolution, _timeSpent, resultType: ResultType.Greedy);
         Solver.InvalidResults.Add(resultError);
      }

      internal override void OnInitializing()
      {
         base.OnInitializing();

         _averageSpeedWalk = Solver.WorkingConfiguration.WalkingSpeed;
         _solutions = new Collection<TOSolution>();
         _currentSolutionGraph = new CityMapGraph();
         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         _timeSpent = DateTime.Now;
         _startPOI = GetStartPOI();
         _startPOI.IsVisited = true;

         if (_startPOI == null)
         {
            throw new OperationCanceledException(nameof(_startPOI));
         }

         _currentSolutionGraph.AddNode(_startPOI.Entity.Id, _startPOI);

         var firstPOIId = _startPOI.Entity.Id;
         var neighborPOI = GetBestNeighbor(_startPOI);
         // Caso particolare descritto nella GetBestNeighbor.
         // Se qua il vicino è null, io ritorno la soluzione così com'è.
         if (neighborPOI == null)
         {
            return;
         }

         neighborPOI.IsVisited = true;
         var neighborPOIId = neighborPOI.Entity.Id;
         _currentSolutionGraph.AddNode(neighborPOIId, neighborPOI);
         RouteWorker edge = _cityMapClone.GetEdge(firstPOIId, neighborPOIId);
         _currentSolutionGraph.AddEdge(firstPOIId, neighborPOIId, edge);
         _newStartPOI = neighborPOI;

         InterestPointWorker GetStartPOI()
         {
            var startPOIId = Solver.WorkingConfiguration.StartPOIId;

            return _cityMapClone.Nodes
               .Where(x => x.Entity.Id == startPOIId)
               .FirstOrDefault();
         }
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
         _cityMapClone = null;
         TOSolution bestProducedSolution = _solutions.Last();
         Result validResult = new Result(bestProducedSolution, _timeSpent, resultType: ResultType.Greedy);
         Solver.ValidResults.Add(validResult);
         Task.WaitAll(Solver.AlgorithmTasks.ToArray());
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         RouteWorker edge = _cityMapClone.GetEdge(_newStartPOI.Entity.Id, _startPOI.Entity.Id);
         if (edge == null)
         {
            throw new InvalidOperationException("There is no edge between nodes");
         }
         _currentSolutionGraph.AddEdge(_newStartPOI.Entity.Id, _startPOI.Entity.Id, edge);
         Solver.BestSolution = _solutions.Last();
      }

      internal override async Task PerformStep()
      {
         var candidatePOI = GetBestNeighbor(_newStartPOI);
         if (candidatePOI == null)
         {
            return;
         }

         candidatePOI.IsVisited = true;
         _currentSolutionGraph.AddNode(candidatePOI.Entity.Id, candidatePOI);
         RouteWorker candidateEdge = _cityMapClone.GetEdge(_newStartPOI.Entity.Id, candidatePOI.Entity.Id);
         _currentSolutionGraph.AddEdge(_newStartPOI.Entity.Id, candidatePOI.Entity.Id, candidateEdge);
         var (tVisit, tWalk, tReturn) = CalculateTimes();
         _newStartPOI = candidatePOI;

         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _currentSolutionGraph,
            TimeSpent = _timeSpent.Add(tWalk)
                                  .Add(tVisit)
                                  .Add(tReturn)
         };
         _solutions.Add(newSolution);

         // Notify observers.
         notifyingFunc.Invoke(newSolution);
         await Task.Delay(500).ConfigureAwait(continueOnCapturedContext: false);

         // Local function: CalculateTimes
         (TimeSpan tVisit, TimeSpan tWalk, TimeSpan tReturn) CalculateTimes()
         {
            TimeSpan timeVisit = default;
            TimeSpan timeWalk = default;
            TimeSpan timeReturn = default;

            if (candidatePOI.Entity.TimeVisit.HasValue)
            {
               timeVisit = candidatePOI.Entity.TimeVisit.Value;
            }

            RouteWorker edge = _cityMapClone.GetEdge(_newStartPOI.Entity.Id, candidatePOI.Entity.Id);
            if (edge == null)
            {
               throw new NullReferenceException(nameof(edge));
            }
            timeWalk = TimeSpan.FromMinutes(edge.Weight() / _averageSpeedWalk / 60);

            RouteWorker returnEdge = _cityMapClone.GetEdge(candidatePOI.Entity.Id, _startPOI.Entity.Id);
            if (returnEdge == null)
            {
               throw new NullReferenceException(nameof(returnEdge));
            }
            timeReturn = TimeSpan.FromMinutes(returnEdge.Weight() / _averageSpeedWalk / 60);

            return (timeVisit, timeWalk, timeReturn);
         }
      }

      internal override bool StopConditions()
      {
         return _currentSolutionGraph.NodeCount == Solver.CityMapGraph.NodeCount ||
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}