﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 07/11/2018
//

using CityScover.Engine.Workers;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// This class implements the Nearest Neighbor algorithm of the Greedy family's algorithms.
   /// </summary>
   internal class NearestNeighbor : GreedyTemplate
   {
      private InterestPointWorker _newStartPOI;

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

         var startingPointId = _startingPoint.Entity.Id;
         _tour.AddNode(startingPointId, _startingPoint);
         var neighborPOI = GetBestNeighbor(_startingPoint);
         if (neighborPOI is null)
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
         if (candidatePOI is null)
         {
            return;
         }

         candidatePOI.IsVisited = true;
         _tour.AddNode(candidatePOI.Entity.Id, candidatePOI);
         SendMessage(MessageCodes.NNPointAdded, candidatePOI.Entity.Name);
         _tour.AddRouteFromGraph(_cityMapClone, _newStartPOI.Entity.Id, candidatePOI.Entity.Id);
         _newStartPOI = candidatePOI;

         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _tour
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
            _tour.NodeCount == _processingNodes.Count();
      }
      #endregion
   }
}