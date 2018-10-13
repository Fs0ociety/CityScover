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
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// This class implements the Nearest Neighbor algorithm of the Greedy family's algorithms.
   /// </summary>
   internal class NearestNeighbor : Algorithm
   {
      #region Private fields
      private double _averageSpeedWalk;
      private ICollection<TOSolution> _solutions;
      protected CityMapGraph _cityMapClone;
      private CityMapGraph _currentSolutionGraph;
      private InterestPointWorker _startPOI;
      private InterestPointWorker _newStartPOI;
      private DateTime _timeSpent;
      #endregion

      #region Constructors
      internal NearestNeighbor()
         : this(null)
      {
      }

      internal NearestNeighbor(AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      private (TimeSpan tVisit, TimeSpan tWalk, TimeSpan tReturn) CalculateTimesByNextPoint(InterestPointWorker point)
      {
         TimeSpan timeVisit = default;
         TimeSpan timeWalk = default;
         TimeSpan timeReturn = default;

         if (point.Entity.TimeVisit.HasValue)
         {
            timeVisit = point.Entity.TimeVisit.Value;
         }

         RouteWorker edge = _cityMapClone.GetEdge(_newStartPOI.Entity.Id, point.Entity.Id);
         if (edge == null)
         {
            throw new NullReferenceException(nameof(edge));
         }
         timeWalk = TimeSpan.FromMinutes(edge.Weight() / _averageSpeedWalk / 60.0);

         RouteWorker returnEdge = _cityMapClone.GetEdge(point.Entity.Id, _startPOI.Entity.Id);
         if (returnEdge == null)
         {
            throw new NullReferenceException(nameof(returnEdge));
         }
         timeReturn = TimeSpan.FromMinutes(returnEdge.Weight() / _averageSpeedWalk / 60.0);

         return (timeVisit, timeWalk, timeReturn);
      }
      #endregion

      #region Protected methods
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

         _currentSolutionGraph.AddRouteFromGraph(_cityMapClone, firstPOIId, neighborPOIId);
         _newStartPOI = neighborPOI;

         InterestPointWorker GetStartPOI()
         {
            var startPOIId = Solver.WorkingConfiguration.StartingPointId;

            return _cityMapClone.Nodes
               .Where(x => x.Entity.Id == startPOIId)
               .FirstOrDefault();
         }
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
         _currentSolutionGraph.AddRouteFromGraph(_cityMapClone, _newStartPOI.Entity.Id, candidatePOI.Entity.Id);
         var (tVisit, tWalk, tReturn) = CalculateTimesByNextPoint(candidatePOI);
         _newStartPOI = candidatePOI;

         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _currentSolutionGraph,
            TimeSpent = _timeSpent.Add(tWalk)
                                  .Add(tVisit)
                                  .Add(tReturn)
         };
         _solutions.Add(newSolution);
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);

         // Notify observers.
         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(newSolution);
         }
      }

      internal override void OnError(Exception exception)
      {
         base.OnError(exception);
         _currentStep = default;
         TOSolution lastProducedSolution = _solutions.Last();
         Result resultError = new Result(lastProducedSolution, _timeSpent, Result.Validity.Invalid);
         Solver.Results.Add(AlgorithmFamily.Greedy, resultError);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         _currentSolutionGraph.AddRouteFromGraph(_cityMapClone, _newStartPOI.Entity.Id, _startPOI.Entity.Id);
         Solver.BestSolution = _solutions.Last();
      }

      internal override void OnTerminated()
      {
         _cityMapClone = null;
         TOSolution bestProducedSolution = _solutions.Last();
         Result validResult = new Result(bestProducedSolution, _timeSpent, Result.Validity.Valid);
         Solver.Results.Add(AlgorithmFamily.Greedy, validResult);
         Task.WaitAll(Solver.AlgorithmTasks.Values.ToArray());
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return 
            _currentSolutionGraph.NodeCount == Solver.CityMapGraph.NodeCount ||
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}