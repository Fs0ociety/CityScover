//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 15/10/2018
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
   /// This class implements the Cheapest Insertion algorithm of the Greedy family's algorithms.
   /// </summary>
   internal class CheapestInsertion : Algorithm
   {
      #region Private fields
      private double _averageSpeedWalk;
      private ICollection<TOSolution> _solutions;
      private CityMapGraph _tour;
      private InterestPointWorker _startingPoint;
      private InterestPointWorker _newStartingPoint;
      private DateTime _timeSpent;
      #endregion

      #region Protected fields
      protected CityMapGraph _cityMapClone;
      #endregion

      #region Constructors
      internal CheapestInsertion()
         : this(null)
      {
      }

      public CheapestInsertion(AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      private InterestPointWorker GetStartPOI()
      {
         var startPOIId = Solver.WorkingConfiguration.StartingPointId;

         return _cityMapClone.Nodes
            .Where(x => x.Entity.Id == startPOIId)
            .FirstOrDefault();
      }

      // ... TODO ...
      #endregion

      #region Protected methods
      protected virtual InterestPointWorker GetFirstBestNeighbor(InterestPointWorker startingPoint)
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;

         var neighbors = _cityMapClone.GetAdjacentNodes(startingPoint.Entity.Id);
         neighbors.ToList().ForEach(neighborId => SetBestCandidate(neighborId));

         void SetBestCandidate(int nodeKey)
         {
            var neighbor = _cityMapClone[nodeKey];
            if (neighbor.IsVisited)
            {
               return;
            }

            int pointScore = neighbor.Entity.Score.Value;
            if (pointScore > bestScore)
            {
               bestScore = pointScore;
               candidateNode = neighbor;
            }
            else if (pointScore == bestScore)
            {
               SetRandomCandidateId(out int pointId);
               candidateNode = _cityMapClone[pointId];
            }

            void SetRandomCandidateId(out int id)
            {
               if (candidateNode == null)
               {
                  id = neighbor.Entity.Id;
               }
               else
               {
                  id = (new Random().Next(2) != 0)
                     ? candidateNode.Entity.Id
                     : neighbor.Entity.Id;
               }
            }
         }

         return candidateNode;
      }

      protected virtual (InterestPointWorker, InterestPointWorker) GetBestNeighbors(InterestPointWorker startingPoint)
      {
         // Tuple version
         //(InterestPointWorker candidateNode, InterestPointWorker candidateNeighbor) candidateNodes = default;
         int bestScore = default;
         InterestPointWorker candidateNode = default;
         InterestPointWorker candidateNeighbor = default;

         // ... TODO ...
         
         return (candidateNode, candidateNeighbor);
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _solutions = new Collection<TOSolution>();
         _tour = new CityMapGraph();
         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         _startingPoint = GetStartPOI();

         if (_startingPoint == null)
         {
            throw new OperationCanceledException(
               $"{nameof(_startingPoint)} in {nameof(CheapestInsertion)}");
         }

         int startingPointId = _startingPoint.Entity.Id;
         _tour.AddNode(startingPointId, _startingPoint);
         _startingPoint.IsVisited = true;
         InterestPointWorker bestNeighbor = GetFirstBestNeighbor(_startingPoint);
         if (bestNeighbor == null)
         {
            return;
         }

         int bestNeighborId = bestNeighbor.Entity.Id;
         _tour.AddNode(bestNeighborId, bestNeighbor);
         bestNeighbor.IsVisited = true;
         _tour.AddRouteFromGraph(_cityMapClone, startingPointId, bestNeighborId);
         _newStartingPoint = _startingPoint;
      }

      internal override async Task PerformStep()
      {
         var (candidateNode, neighbor) = GetBestNeighbors(_newStartingPoint);
         if (candidateNode == null || neighbor == null)
         {
            return;
         }

         // ... TODO ... Insert the candidaNode between _newStartingPoint and neighbor in the _tour graph.

         await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);
         // ... TODO ...
      }

      internal override void OnError(Exception exception)
      {
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return _tour.NodeCount == _cityMapClone.NodeCount ||
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}
