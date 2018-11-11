//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 11/11/2018
//

using CityScover.Commons;
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
      private int _previousCandidateId;

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
         _tour.AddRouteFromGraph(_cityMapClone, neighborPOIId, startingPointId);
         _previousCandidateId = neighborPOI.Entity.Id;
      }

      internal override async Task PerformStep()
      {
         _tour.RemoveEdge(_previousCandidateId, _startingPoint.Entity.Id);
         InterestPointWorker newStartPOI = _cityMapClone[_processingNodes.Dequeue()];

         var candidatePOI = GetBestNeighbor(newStartPOI);
         if (candidatePOI is null)
         {
            _tour.AddRouteFromGraph(_cityMapClone, _previousCandidateId, _startingPoint.Entity.Id);
            return;
         }
         
         candidatePOI.IsVisited = true;
         _tour.AddNode(candidatePOI.Entity.Id, candidatePOI);
         _tour.AddRouteFromGraph(_cityMapClone, _previousCandidateId, candidatePOI.Entity.Id);
         _tour.AddRouteFromGraph(_cityMapClone, candidatePOI.Entity.Id, _startingPoint.Entity.Id);
         
         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = _tour.DeepCopy()
         };

         SendMessage(MessageCode.GreedyNodeAdded, candidatePOI.Entity.Name, newSolution.Id);
         _solutions.Add(newSolution);
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[newSolution.Id];

         if (!newSolution.IsValid)
         {
            _tour.RemoveEdge(_previousCandidateId, candidatePOI.Entity.Id);
            _tour.RemoveEdge(candidatePOI.Entity.Id, _startingPoint.Entity.Id);
            _tour.RemoveNode(candidatePOI.Entity.Id);
            _tour.AddRouteFromGraph(_cityMapClone, _previousCandidateId, _startingPoint.Entity.Id);
            SendMessage(MessageCode.GreedyNodeRemoved, candidatePOI.Entity.Name);
         }
         else
         {
            _previousCandidateId = candidatePOI.Entity.Id;
         }

         // Notify observers.
         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(newSolution);
         }
      }

      internal override bool StopConditions()
      {
         return base.StopConditions() || !_processingNodes.Any();
      }
      #endregion
   }
}