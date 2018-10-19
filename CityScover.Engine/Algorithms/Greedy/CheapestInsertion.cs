//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 19/10/2018
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
      private InterestPointWorker _finalPoint;
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

      private (TimeSpan, TimeSpan, TimeSpan) CalculateTimesByNextPoint(InterestPointWorker point)
      {
         TimeSpan timeVisit = default;
         TimeSpan timeWalk = default;
         TimeSpan timeReturn = default;

         if (point.Entity.TimeVisit.HasValue)
         {
            timeVisit = point.Entity.TimeVisit.Value;
         }

         RouteWorker edge = _cityMapClone.GetEdge(_newStartingPoint.Entity.Id, point.Entity.Id);
         if (edge == null)
         {
            throw new NullReferenceException(nameof(edge));
         }

         double averageSpeedWalk = _averageSpeedWalk / 60.0;
         timeWalk = TimeSpan.FromMinutes(edge.Weight() / averageSpeedWalk);

         RouteWorker returnEdge = _cityMapClone.GetEdge(point.Entity.Id, _startingPoint.Entity.Id);
         if (returnEdge == null)
         {
            throw new NullReferenceException(nameof(returnEdge));
         }
         timeReturn = TimeSpan.FromMinutes(returnEdge.Weight() / averageSpeedWalk);

         return (timeVisit, timeWalk, timeReturn);
      }
      #endregion

      #region Protected methods
      protected virtual InterestPointWorker GetFirstBestNeighbor(InterestPointWorker interestPoint)
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;

         var neighbors = _cityMapClone.GetAdjacentNodes(interestPoint.Entity.Id);
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

      protected virtual InterestPointWorker GetBestNeighbor()
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;
         Collection<InterestPointWorker> candidateNodes = new Collection<InterestPointWorker>();

         foreach (var currentPoint in _tour.Nodes)
         {
            int currentPointId = currentPoint.Entity.Id;
            int currentPointScore = currentPoint.Entity.Score.Value;
            var neighborPoints = _tour.GetAdjacentNodes(currentPointId);

            foreach (var neighborPointId in neighborPoints)    // Valutare uso di ForEach di LINQ
            {
               var processingEdge = _cityMapClone.GetEdge(currentPointId, neighborPointId);
               if (processingEdge == null)
               {
                  throw new NullReferenceException(nameof(processingEdge));
               }

               InterestPointWorker neighborPoint = _cityMapClone[neighborPointId];
               int neighborPointScore = neighborPoint.Entity.Score.Value;
               int currPointToNeighborScore = Math.Abs(currentPointScore - neighborPointScore);

               foreach (var node in _cityMapClone.Nodes)
               {
                  int nodeId = node.Entity.Id;
                  int nodeScore = node.Entity.Score.Value;
                  bool canCompareCosts = !_tour.ContainsNode(nodeId) && nodeId != neighborPointId && !node.IsVisited;

                  if (canCompareCosts)
                  {
                     int currPointToNodeScore = Math.Abs(currentPointScore - nodeScore);
                     int nodeToNeighborScore = Math.Abs(nodeScore - neighborPointScore);
                     int deltaScore = currPointToNodeScore + nodeToNeighborScore - currPointToNeighborScore;

                     if (deltaScore > bestScore)
                     {
                        bestScore = deltaScore;
                        candidateNode = node;
                     }
                     else if (deltaScore == 0 || deltaScore == bestScore)
                     {
                        candidateNodes.Add(node);
                     }
                  }
               }
               processingEdge.IsVisited = true;

               bool hasToBeSettedCandidate = candidateNode == null || 
                  (candidateNode != null && candidateNodes.Any());

               if (hasToBeSettedCandidate)
               {
                  var candidateIndex = new Random().Next(0, candidateNodes.Count);
                  candidateNode = candidateNodes[candidateIndex];
                  candidateNodes.Clear();
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
         _tour = new CityMapGraph();
         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         _timeSpent = DateTime.Now;
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
         _finalPoint = bestNeighbor;
      }

      internal override async Task PerformStep()
      {
         InterestPointWorker candidateNode = GetBestNeighbor();
         if (candidateNode == null)
         {
            return;
         }

         _tour.AddNode(candidateNode.Entity.Id, candidateNode);
         _tour.AddRouteFromGraph(_cityMapClone, _newStartingPoint.Entity.Id, candidateNode.Entity.Id);
         _tour.AddRouteFromGraph(_cityMapClone, candidateNode.Entity.Id, _finalPoint.Entity.Id);
         _tour.RemoveEdge(_newStartingPoint.Entity.Id, _finalPoint.Entity.Id);
         var (tVisit, tWalk, tReturn) = CalculateTimesByNextPoint(candidateNode);
         _newStartingPoint = candidateNode;

         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _tour,
            TimeSpent = _timeSpent
                        .Add(tWalk)
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
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         _tour.AddRouteFromGraph(_cityMapClone, _finalPoint.Entity.Id, _startingPoint.Entity.Id);
         Solver.BestSolution = _solutions.Last();
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
