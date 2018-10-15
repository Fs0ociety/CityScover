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
      private ICollection<TOSolution> _solutions;
      private CityMapGraph _currentSolutionGraph;
      private InterestPointWorker _startingPoint;
      private InterestPointWorker _newStartingPoint;
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
      // ... TODO ...
      #endregion

      #region Protected methods
      protected virtual InterestPointWorker GetBestNeighbor(InterestPointWorker startingPoint)
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _solutions = new Collection<TOSolution>();
         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         _currentSolutionGraph = new CityMapGraph();
         _startingPoint = GetStartPOI();

         if (_startingPoint == null)
         {
            throw new OperationCanceledException(
               $"{nameof(_startingPoint)} in {nameof(CheapestInsertion)}");
         }

         _startingPoint.IsVisited = true;
         int startingPointId = _startingPoint.Entity.Id;
         _currentSolutionGraph.AddNode(startingPointId, _startingPoint);
         InterestPointWorker bestNeighbor = GetBestNeighbor(_startingPoint);

         if (bestNeighbor == null)
         {
            return;
         }

         bestNeighbor.IsVisited = true;
         int bestNeighborId = bestNeighbor.Entity.Id;
         _currentSolutionGraph.AddNode(bestNeighborId, bestNeighbor);
         _currentSolutionGraph.AddRouteFromGraph(_cityMapClone, startingPointId, bestNeighborId);
         _newStartingPoint = _startingPoint;

         InterestPointWorker GetStartPOI()
         {
            var startPOIId = Solver.WorkingConfiguration.StartingPointId;

            return _cityMapClone.Nodes
               .Where(x => x.Entity.Id == startPOIId)
               .FirstOrDefault();
         }
      }

      internal override Task PerformStep()
      {
         throw new NotImplementedException();
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
         return _currentSolutionGraph.NodeCount == _cityMapClone.NodeCount ||
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}
