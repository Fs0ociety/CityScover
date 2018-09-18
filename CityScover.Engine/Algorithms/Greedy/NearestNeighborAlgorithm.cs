//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/09/2018
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
      private CityMapGraph _currentSolution;
      private InterestPointWorker _startPOI;
      private InterestPointWorker _newStartPOI;

      #region Constructors
      internal NearestNeighborAlgorithm()
         : base()
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

         var adjPOIIds = Solver.CityMapGraph.GetAdjacentNodes(interestPoint.Entity.Id);
         var cityMapGraphNodes = Solver.CityMapGraph.Nodes;

         adjPOIIds.ToList().ForEach(adjPOIId => SetBestCandidate(adjPOIId));

         void SetBestCandidate(int nodeKey)
         {
            var node = Solver.CityMapGraph[nodeKey];

            if (!node.IsVisited)
            {
               var deltaScore = Math.Abs(node.Entity.Score.Value -
                  interestPoint.Entity.Score.Value);

               if (deltaScore > bestScore)
               {
                  bestScore = deltaScore;
                  candidateNode = node;
               }
            }
         }

         #region Old-style code
         //foreach (var adjPOIId in adjPOIIds)
         //{
         //   var adjPOI = Solver.CityMapGraph[adjPOIId];

         //   if (!adjPOI.IsVisited)
         //   {
         //      var deltaScore = Math.Abs(adjPOI.Entity.Score.Value -
         //         interestPoint.Entity.Score.Value);

         //      if (deltaScore > bestScore)
         //      {
         //         bestScore = deltaScore;
         //         candidateNode = adjPOI;
         //      }
         //   }
         //}
         #endregion

         return candidateNode;
      }

      private void ResetCityMapGraph()
      {
         Solver.CityMapGraph.Nodes
            .Where(node => node.IsVisited == true)
            .ToList()
            .ForEach(node => node.IsVisited = false);
      }
      #endregion

      #region Overrides
      internal override void OnError()
      {
         ResetCityMapGraph();
         _currentStep = default;
         // TODO: Other activities?
      }

      internal override void OnInitializing()
      {
         _currentSolution = new CityMapGraph();
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

         var edge = Solver.CityMapGraph.GetEdge(firstPOIId, neighborPOIId);
         _currentSolution.AddUndirectedEdge(firstPOIId, neighborPOIId, edge);
         _newStartPOI = _startPOI;

         InterestPointWorker GetStartPOI()
         {
            var startPOIId = Solver.WorkingConfiguration.StartPOIId;

            return Solver.CityMapGraph.Nodes
               .Where(x => x.Entity.Id == startPOIId)
               .FirstOrDefault();
         }
      }

      internal override void OnTerminated()
      {
         _currentStep = default;
         //throw new NotImplementedException();
      }

      internal override void OnTerminating()
      {
         ResetCityMapGraph();
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
            _newStartPOI = candidatePOI;

            TOSolution newSolution = new TOSolution
            {
               SolutionGraph = _currentSolution
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