//
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
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// This class implements the Nearest Neighbor algorithm of the Greedy family's algorithms.
   /// </summary>
   internal class NearestNeighbor : GreedyTemplate
   {
      #region Private fields
      private InterestPointWorker _newStartPOI;
      #endregion

      #region Private methods
      private (TimeSpan, TimeSpan, TimeSpan) CalculateTimesByNextPoint(InterestPointWorker point)
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

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

         var startingPointId = _startingPoint.Entity.Id;
         _tour.AddNode(startingPointId, _startingPoint);
         var neighborPOI = GetBestNeighbor(_startingPoint);
         if (neighborPOI == null)
         {
            return;
         }

         neighborPOI.IsVisited = true;
         var neighborPOIId = neighborPOI.Entity.Id;
         _tour.AddNode(neighborPOIId, neighborPOI);
         _tour.AddRouteFromGraph(_cityMapClone, startingPointId, neighborPOIId);
         _newStartPOI = neighborPOI;
      }

      internal override async Task PerformStep()
      {
         var candidatePOI = GetBestNeighbor(_newStartPOI);
         if (candidatePOI == null)
         {
            return;
         }

         candidatePOI.IsVisited = true;
         _tour.AddNode(candidatePOI.Entity.Id, candidatePOI);
         _tour.AddRouteFromGraph(_cityMapClone, _newStartPOI.Entity.Id, candidatePOI.Entity.Id);
         _newStartPOI = candidatePOI;
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
         _tour.AddRouteFromGraph(_cityMapClone, _newStartPOI.Entity.Id, _startingPoint.Entity.Id);
      }

      internal override bool StopConditions()
      {
         return base.StopConditions() || 
            _tour.NodeCount == _cityMapClone.NodeCount;
      }
      #endregion
   }
}