//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 19/09/2018
//

using CityScover.Engine.Workers;
using System;
using System.Linq;

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// TODO
   /// </summary>
   internal class NearestNeighborAlgorithm : Algorithm
   {
      // A temporary variable for average speed walk (it must be added to repository!!!)
      // Its measure unit is km/h.
      private const double _averageSpeedWalk = 3.0;

      private CityMapGraph _currentSolution;
      private CityMapGraph _cityMapClone;

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
      private InterestPointWorker GetClosestNeighborByScore(
         InterestPointWorker interestPoint)
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;

         var adjPOIIds = _cityMapClone.GetAdjacentNodes(interestPoint.Entity.Id);
         var cityMapGraphNodes = _cityMapClone.Nodes;

         adjPOIIds.ToList().ForEach(adjPOIId => SetBestCandidate(adjPOIId));

         void SetBestCandidate(int nodeKey)
         {
            var node = _cityMapClone[nodeKey];

            if (!node.IsVisited)
            {
               var deltaScore = Math.Abs(node.Entity.Score.Value -
                  interestPoint.Entity.Score.Value);

               if (deltaScore > bestScore)
               {
                  bestScore = deltaScore;
                  candidateNode = node;
               }
               else if (deltaScore == bestScore)
               {
                  var randomId = new Random().Next(interestPoint.Entity.Id, node.Entity.Id);
                  candidateNode = _cityMapClone[randomId];
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
         // TODO: Other activities?
      }

      internal override void OnInitializing()         
      {
         base.OnInitializing();

         _currentSolution = new CityMapGraph();
         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         _timeSpent = DateTime.Now;
         _startPOI = GetStartPOI();
         _startPOI.IsVisited = true;
         
         if (_startPOI == null)
         {
            throw new OperationCanceledException(nameof(_startPOI));
         }

         _currentSolution.AddNode(_startPOI.Entity.Id, _startPOI);

         var firstPOIId = _startPOI.Entity.Id;
         var neighborPOI = GetClosestNeighborByScore(_startPOI);
         neighborPOI.IsVisited = true;
         var neighborPOIId = neighborPOI.Entity.Id;
         _currentSolution.AddNode(neighborPOIId, neighborPOI);
         
         var edge = _cityMapClone.GetEdge(firstPOIId, neighborPOIId);
         _currentSolution.AddUndirectedEdge(firstPOIId, neighborPOIId, edge);
         _newStartPOI = _startPOI;

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
         _currentStep = default;
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         // TODO: Other activities?
      }

      internal override void PerformStep()
      {
         var candidatePOI = GetClosestNeighborByScore(_newStartPOI);

         if (candidatePOI != null)
         {
            candidatePOI.IsVisited = true;
            _currentSolution.AddNode(candidatePOI.Entity.Id, candidatePOI);
            _currentSolution.AddUndirectedEdge(_newStartPOI.Entity.Id, candidatePOI.Entity.Id);
            var (tVisit, tWalk, tReturn) = CalculateTimes();
            _newStartPOI = candidatePOI;

            (TimeSpan tVisit, TimeSpan tWalk, TimeSpan tReturn) CalculateTimes()
            {
               TimeSpan timeVisit = default;
               if (candidatePOI.Entity.TimeVisit.HasValue)
               {
                  timeVisit = candidatePOI.Entity.TimeVisit.Value;
               }

               RouteWorker edge = _cityMapClone.GetEdge(_newStartPOI.Entity.Id, candidatePOI.Entity.Id);
               TimeSpan timeWalk = TimeSpan.FromHours(edge.Weight() / _averageSpeedWalk);

               RouteWorker returnEdge = _cityMapClone.GetEdge(candidatePOI.Entity.Id, _startPOI.Entity.Id);
               TimeSpan timeReturn = TimeSpan.FromHours(returnEdge.Weight() / _averageSpeedWalk);

               return (timeVisit, timeWalk, timeReturn);
            }

            TOSolution newSolution = new TOSolution
            {
               SolutionGraph = _currentSolution,
               TimeSpent = _timeSpent.Add(tWalk)
                                     .Add(tVisit)
                                     .Add(tReturn)
            };

            if (Solver.IsMonitoringEnabled)
            {
               Provider.NotifyObservers(newSolution);
            }
            else
            {
               Solver.SolutionsQueue.Add(newSolution);
            }
         }
         else
         {
            // TODO: Gestire diversamente la continuazione dell'algoritmo.
            // Ad esempio:
            // selezione casuale di un nuovo nodo candidato nel caso in cui i punteggi coincidano.
            _status = AlgorithmStatus.Terminating;
         }
      }

      internal override bool StopConditions()
      {
         return _currentSolution.NodeCount == Solver.CityMapGraph.NodeCount ||
            _status == AlgorithmStatus.Terminating;
      }
      #endregion
   }
}