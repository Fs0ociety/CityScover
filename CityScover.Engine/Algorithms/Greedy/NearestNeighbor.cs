//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/11/2018
//

using CityScover.Commons;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Greedy
{
   /// <summary>
   /// This class implements the Nearest Neighbor algorithm of the Greedy family's algorithms.
   /// </summary>
   internal class NearestNeighbor : GreedyTemplate
   {
      private int _previousCandidateId;

      #region Constructors
      internal NearestNeighbor()
         : this(provider: null)
      {
      }

      internal NearestNeighbor(AlgorithmTracker provider)
         : base(provider)
      {
         Type = AlgorithmType.NearestNeighbor;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         var startingPointId = StartingPoint.Entity.Id;
         Tour.AddNode(startingPointId, StartingPoint);
         var neighborPoi = GetBestNeighbor(StartingPoint);
         if (neighborPoi is null)
         {
            return;
         }

         neighborPoi.IsVisited = true;
         var neighborPoiId = neighborPoi.Entity.Id;
         Tour.AddNode(neighborPoiId, neighborPoi);
         Tour.AddRouteFromGraph(CityMapClone, startingPointId, neighborPoiId);
         Tour.AddRouteFromGraph(CityMapClone, neighborPoiId, startingPointId);
         _previousCandidateId = neighborPoi.Entity.Id;
      }

      internal override async Task PerformStep()
      {
         Tour.RemoveEdge(_previousCandidateId, StartingPoint.Entity.Id);
         var newStartPoi = CityMapClone[ProcessingNodes.Dequeue()];

         var candidatePoi = GetBestNeighbor(newStartPoi);
         if (candidatePoi is null)
         {
            Tour.AddRouteFromGraph(CityMapClone, _previousCandidateId, StartingPoint.Entity.Id);
            return;
         }
         
         candidatePoi.IsVisited = true;
         Tour.AddNode(candidatePoi.Entity.Id, candidatePoi);
         Tour.AddRouteFromGraph(CityMapClone, _previousCandidateId, candidatePoi.Entity.Id);
         Tour.AddRouteFromGraph(CityMapClone, candidatePoi.Entity.Id, StartingPoint.Entity.Id);
         
         TOSolution newSolution = new TOSolution()
         {
            SolutionGraph = Tour.DeepCopy()
         };

         SendMessage(MessageCode.GreedyNodeAdded, candidatePoi.Entity.Name, newSolution.Id);
         SolutionsHistory.Add(newSolution);
         Solver.EnqueueSolution(newSolution);
         await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[newSolution.Id];

         if (!newSolution.IsValid)
         {
            Tour.RemoveEdge(_previousCandidateId, candidatePoi.Entity.Id);
            Tour.RemoveEdge(candidatePoi.Entity.Id, StartingPoint.Entity.Id);
            Tour.RemoveNode(candidatePoi.Entity.Id);
            Tour.AddRouteFromGraph(CityMapClone, _previousCandidateId, StartingPoint.Entity.Id);
            SendMessage(MessageCode.GreedyNodeRemoved, candidatePoi.Entity.Name);
         }
         else
         {
            _previousCandidateId = candidatePoi.Entity.Id;
         }

         // Notify observers.
         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(newSolution);
         }
      }
      #endregion
   }
}