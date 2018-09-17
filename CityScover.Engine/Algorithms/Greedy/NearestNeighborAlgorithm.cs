//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 08/09/2018
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
      private int _currentSolutionId = default;  // TODO: gestire l'ID nella TOSolution
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

         foreach (var adjPOIId in adjPOIIds)
         {
            var adjPOI = Solver.CityMapGraph[adjPOIId];
            var deltaScore = Math.Abs(adjPOI.Entity.Score.Value - 
               interestPoint.Entity.Score.Value);

            if (deltaScore > bestScore)
            {
               bestScore = deltaScore;
               candidateNode = adjPOI;
            }
         }

         return candidateNode;
      }
      #endregion

      #region Overrides
      internal override void OnError()
      {
         throw new NotImplementedException();
      }

      internal override void OnInitializing()
      {
         _currentSolution = new CityMapGraph();

         _startPOI = GetStartPOI();
         _startPOI.IsVisited = true;

         InterestPointWorker GetStartPOI()
         {
            var cityMapGraphNodes = Solver.CityMapGraph.Nodes;
            var startPOIId = Solver.WorkingConfiguration.StartPOIId;
            return cityMapGraphNodes.Where(x => x.Entity.Id == startPOIId).FirstOrDefault();
         }
         
         if (_startPOI == null)
         {
            //TODO Gestire l'eccezione.
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
      }

      internal override void OnTerminated()
      {
         throw new NotImplementedException();
      }

      internal override void OnTerminating()
      {
         throw new NotImplementedException();
      }

      internal override void PerformStep()
      {
         var newSolution = new TOSolution
         {
            Id = ++_currentSolutionId
         };

         var candidatePOI = GetClosestNeighborByScore(_newStartPOI);
         candidatePOI.IsVisited = true;

         newSolution.SolutionGraph.AddNode(candidatePOI.Entity.Id, candidatePOI);
         newSolution.SolutionGraph.AddUndirectedEdge(_newStartPOI.Entity.Id, candidatePOI.Entity.Id);
         _newStartPOI = candidatePOI;

         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(newSolution);
         }
         else
         {
            Solver.SolutionsQueue.Add(newSolution);
         }
      }

      internal override bool StopConditions()
      {
         return _currentSolution.NodeCount < Solver.CityMapGraph.NodeCount;
      } 
      #endregion
   }
}
