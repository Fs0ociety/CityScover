﻿//
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

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// This class implements the Cheapest Insertion algorithm of the Greedy family's algorithms.
   /// </summary>
   internal class CheapestInsertion : GreedyTemplate
   {
      #region Private fields
      private InterestPointWorker _newStartingPoint;
      private InterestPointWorker _endPoint;
      #endregion

      #region Private methods
      private InterestPointWorker GetCheapestBestNeighbor()
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;
         Collection<InterestPointWorker> candidateNodes = new Collection<InterestPointWorker>();

         foreach (var currentPoint in _tour.Nodes)
         {
            int currentPointId = currentPoint.Entity.Id;
            int currentPointScore = currentPoint.Entity.Score.Value;
            var neighborPoints = _tour.GetAdjacentNodes(currentPointId);

            foreach (var neighborPointId in neighborPoints)
            {
               var processingEdge = _cityMapClone.GetEdge(currentPointId, neighborPointId);
               if (processingEdge == null)
               {
                  return null;
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

               if (HasToBeSettedCandidateNode(candidateNode, candidateNodes))
               {
                  var nodeIndex = new Random().Next(0, candidateNodes.Count);
                  candidateNode = candidateNodes[nodeIndex];
                  candidateNodes.Clear();
               }
            }
         }
      
         return candidateNode;
      }

      private bool HasToBeSettedCandidateNode(InterestPointWorker candidateNode, IEnumerable<InterestPointWorker> candidateNodes)
      {
         return candidateNode == null || (candidateNode != null && candidateNodes.Any());
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

         int startingPointId = _startingPoint.Entity.Id;
         _tour.AddNode(startingPointId, _startingPoint);
         InterestPointWorker bestNeighbor = GetBestNeighbor(_startingPoint);
         if (bestNeighbor == null)
         {
            return;
         }

         int bestNeighborId = bestNeighbor.Entity.Id;
         _tour.AddNode(bestNeighborId, bestNeighbor);
         bestNeighbor.IsVisited = true;
         _tour.AddRouteFromGraph(_cityMapClone, startingPointId, bestNeighborId);
         _newStartingPoint = _startingPoint;
         _endPoint = bestNeighbor;
      }

      internal override async Task PerformStep()
      {
         InterestPointWorker candidateNode = GetCheapestBestNeighbor();
         if (candidateNode == null)
         {
            return;
         }

         _tour.AddNode(candidateNode.Entity.Id, candidateNode);
         _tour.AddRouteFromGraph(_cityMapClone, _newStartingPoint.Entity.Id, candidateNode.Entity.Id);
         _tour.AddRouteFromGraph(_cityMapClone, candidateNode.Entity.Id, _endPoint.Entity.Id);
         _tour.RemoveEdge(_newStartingPoint.Entity.Id, _endPoint.Entity.Id);
         _newStartingPoint = candidateNode;
         _tour.CalculateTimes();

         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _tour,
            TimeSpent = GetTotalTime()
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

      internal override void OnTerminating()
      {
         base.OnTerminating();
         _tour.AddRouteFromGraph(_cityMapClone, _endPoint.Entity.Id, _startingPoint.Entity.Id);
      }

      internal override bool StopConditions()
      {
         return base.StopConditions() || 
            _tour.NodeCount == _cityMapClone.NodeCount;
      }
      #endregion
   }
}