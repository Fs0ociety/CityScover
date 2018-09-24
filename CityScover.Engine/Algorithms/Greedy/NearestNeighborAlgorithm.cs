﻿//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/09/2018
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
      private float _averageSpeedWalk;

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
      /// <summary>
      /// Returns the best candidate node near to point of interest passed as argument.
      /// </summary>
      /// <param name="interestPoint">Point of interest</param>
      /// <returns></returns>
      private InterestPointWorker GetClosestNeighborByScore(InterestPointWorker interestPoint)
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;

         var adjPOIIds = _cityMapClone.GetAdjacentNodes(interestPoint.Entity.Id);
         var cityMapGraphNodes = _cityMapClone.Nodes;

         adjPOIIds.ToList().ForEach(adjPOIId => SetBestCandidate(adjPOIId));

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
               if (interestPoint.Entity.Id < node.Entity.Id)
               {
                  id = new Random().Next(interestPoint.Entity.Id, node.Entity.Id);
               }
               else
               {
                  id = new Random().Next(node.Entity.Id, interestPoint.Entity.Id);
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
         Result resultError = new Result(_currentSolution, _currentStep, _timeSpent);
         // Solver.InvalidResults.Add(resultError)

         // TODO: Other activities?
      }

      internal override void OnInitializing()
      {
         base.OnInitializing();

         _averageSpeedWalk = Solver.WorkingConfiguration.WalkingSpeed;
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
         Result validResult = new Result(_currentSolution, _currentStep, _timeSpent);
         // Solver.ValidResults.Add(validResult)
         _currentStep = default;

         // TODO: Other activities?
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         _cityMapClone = null;
         // TODO: Other activities?
      }

      internal override void PerformStep()
      {
         var candidatePOI = GetClosestNeighborByScore(_newStartPOI);

         candidatePOI.IsVisited = true;
         _currentSolution.AddNode(candidatePOI.Entity.Id, candidatePOI);
         _currentSolution.AddUndirectedEdge(_newStartPOI.Entity.Id, candidatePOI.Entity.Id);
         var (tVisit, tWalk, tReturn) = CalculateTimes();
         _newStartPOI = candidatePOI;

         TOSolution newSolution = new TOSolution
         {
            SolutionGraph = _currentSolution,
            TimeSpent = _timeSpent.Add(tWalk)
                                  .Add(tVisit)
                                  .Add(tReturn)
         };

         // Notify observers.
         notifyingFunc.Invoke(newSolution);
         
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
            timeWalk = TimeSpan.FromHours(edge.Weight() / _averageSpeedWalk);

            RouteWorker returnEdge = _cityMapClone.GetEdge(candidatePOI.Entity.Id, _startPOI.Entity.Id);
            if (returnEdge == null)
            {
               throw new NullReferenceException(nameof(returnEdge));
            }
            timeReturn = TimeSpan.FromHours(returnEdge.Weight() / _averageSpeedWalk);

            return (timeVisit, timeWalk, timeReturn);
         }
      }

      internal override bool StopConditions()
      {
         return _currentSolution.NodeCount == Solver.CityMapGraph.NodeCount ||
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}