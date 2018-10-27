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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.CustomAlgorithms
{
   internal class HybridNearestDistance : Algorithm
   {
      #region Private fields
      private TOSolution _currentSolution;
      private CityMapGraph _currentSolutionGraph;
      private InterestPointWorker _startPOI;
      private InterestPointWorker _endPOI;
      private Queue<InterestPointWorker> _processingNodes;
      private TimeSpan _tMaxThreshold;
      private DateTime _tMax;
      private int _addedNodes;
      #endregion

      #region Constructors
      internal HybridNearestDistance()
         : this(null)
      {
      }

      internal HybridNearestDistance(AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Private Methods
      private IEnumerable<InterestPointWorker> GetPointsNotInTour()
      {
         IEnumerable<InterestPointWorker> cityMapGraphNodes = Solver.CityMapGraph.Nodes;
         IEnumerable<InterestPointWorker> currentSolutionNodes = _currentSolutionGraph.Nodes;
         IEnumerable<InterestPointWorker> nodes = cityMapGraphNodes.Except(currentSolutionNodes);
         return nodes;
      }

      private void TryInsertNode(InterestPointWorker point)
      {
         _currentSolutionGraph.AddNode(point.Entity.Id, point);
         _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, _endPOI.Entity.Id, point.Entity.Id);
         _currentSolutionGraph.RemoveEdge(_endPOI.Entity.Id, _startPOI.Entity.Id);
         _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, point.Entity.Id, _startPOI.Entity.Id);
      }

      private void UndoAdditionPoint(InterestPointWorker point)
      {
         _currentSolutionGraph.RemoveEdge(point.Entity.Id, _startPOI.Entity.Id);
         _currentSolutionGraph.RemoveEdge(_endPOI.Entity.Id, point.Entity.Id);
         _currentSolutionGraph.RemoveNode(point.Entity.Id);
         _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, _endPOI.Entity.Id, _startPOI.Entity.Id);
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _processingNodes = new Queue<InterestPointWorker>();
         TOSolution bestSolution = Solver.BestSolution;
         Solver.PreviousStageSolutionCost = bestSolution.Cost;
         _currentSolutionGraph = bestSolution.SolutionGraph.DeepCopy();
         _tMaxThreshold = Solver.CurrentStage.Flow.HndTmaxThreshold;
         _tMax = Solver.WorkingConfiguration.ArrivalTime.Add(Solver.WorkingConfiguration.TourDuration);
         _startPOI = _currentSolutionGraph.GetStartPoint();
         _endPOI = _currentSolutionGraph.GetEndPoint();

         if (_startPOI == null || _endPOI == null)
         {
            throw new NullReferenceException();
         }

         GetPointsNotInTour().ToList().ForEach(point => _processingNodes.Enqueue(point));
         _addedNodes = _processingNodes.Count;
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();

         if (_addedNodes == 0)
         {
            // TODO: invocare secondo step.
            //Task algorithmTask = Task.Run(() => );
            //Task.WaitAll(algorithmTask);
         }

         bool isBetterThanCurrentBestSolution = 
            Solver.Problem.CompareSolutionsCost(_currentSolution.Cost, Solver.BestSolution.Cost);
         if (isBetterThanCurrentBestSolution)
         {
            Solver.BestSolution = _currentSolution;
         }
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override async Task PerformStep()
      {
         InterestPointWorker point = _processingNodes.Dequeue();
         TryInsertNode(point);

         _currentSolution = new TOSolution()
         {
            SolutionGraph = _currentSolutionGraph
         };

         Solver.EnqueueSolution(_currentSolution);
         await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);
         await Solver.AlgorithmTasks[_currentSolution.Id];

         if (!_currentSolution.IsValid)
         {
            UndoAdditionPoint(point);
            _addedNodes--;
         }
      }

      internal override void OnError(Exception exception)
      {
         base.OnError(exception);
      }

      internal override bool StopConditions()
      {
         InterestPointWorker endPOI = _currentSolutionGraph.GetEndPoint();
         TimeSpan availableTime = _tMax.Subtract(endPOI.TotalTime);
         bool isGreaterThanTmaxThreshold = availableTime > _tMaxThreshold;
         bool shouldStop = isGreaterThanTmaxThreshold || 
            _processingNodes.Count() == 0 || 
            _status == AlgorithmStatus.Error;

         return shouldStop;
      }
      #endregion
   }
}