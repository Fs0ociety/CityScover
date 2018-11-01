//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 01/11/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.CustomAlgorithms
{
   internal class HybridNearestDistanceInsertion : Algorithm
   {
      #region Private fields
      private int _addedNodesCount;
      private bool _isTourUpdated;
      private InterestPointWorker _startPOI;
      private InterestPointWorker _endPOI;
      private DateTime _tMax;
      private TimeSpan _tMaxThreshold;
      private TimeSpan _timeWalkThreshold;
      private TOSolution _currentSolution;
      private CityMapGraph _currentSolutionGraph;
      private Queue<InterestPointWorker> _processingNodes;
      #endregion

      #region Constructors
      internal HybridNearestDistanceInsertion()
         : this(null)
      {
      }

      internal HybridNearestDistanceInsertion(AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      private IEnumerable<InterestPointWorker> GetPointsNotInTour()
      {
         IEnumerable<InterestPointWorker> cityMapGraphNodes = Solver.CityMapGraph.Nodes;
         IEnumerable<InterestPointWorker> currentSolutionNodes = _currentSolutionGraph.Nodes;

         return cityMapGraphNodes.Except(currentSolutionNodes);
      }

      private void AddPointsNotInCurrentTour() =>
         GetPointsNotInTour().ToList().ForEach(point => _processingNodes.Enqueue(point));

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

      private IEnumerable<(RouteWorker edge, TimeSpan tWalk)> CalculateMaximumEdgesTimeWalk(double averageSpeedWalk)
      {
         // Insieme delle tuple contenenti le informazioni: (Tratta, Tempo di percorrenza).
         Collection<(RouteWorker, TimeSpan)> removalEdgesCandidates = new Collection<(RouteWorker, TimeSpan)>();

         foreach (var route in _currentSolutionGraph.Edges)
         {
            // Tempo di percorrenza dell'arco in minuti.
            double tWalkMinutes = (route.Weight() / averageSpeedWalk) / 60.0;
            TimeSpan timeRouteWalk = TimeSpan.FromMinutes(tWalkMinutes);

            if (timeRouteWalk > _timeWalkThreshold)
            {
               var removalEdgeCandidate = (route, timeRouteWalk);
               removalEdgesCandidates.Add(removalEdgeCandidate);
            }
         }

         return removalEdgesCandidates;
      }

      private async Task TryUpdateTour_v1()
      {
         double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed;
         var removalEdgesCandidates = CalculateMaximumEdgesTimeWalk(averageSpeedWalk);

         if (!removalEdgesCandidates.Any())
         {
            return;
         }
         AddPointsNotInCurrentTour();
         _processingNodes.OrderByDescending(point => point.Entity.Score.Value);
      
         InterestPointWorker tourPointToRemove = default;
         InterestPointWorker predecessorPoint = default;
         RouteWorker ingoingRoute = default;
         int currentPointScore = int.MaxValue;
         bool tourUpdated = default;

         foreach (var node in _processingNodes)
         {
            if (tourUpdated)
            {
               // Need to recalculate the list of edges of new updated tour,
               // because old edges have been removed from the tour.
               //removalEdgesCandidates = CalculateMaximumEdgesTimeWalk(averageSpeedWalk);
            }

            foreach (var (edge, tWalk) in removalEdgesCandidates)
            {
               InterestPointWorker currentPointFrom = _currentSolutionGraph.TourPoints
                  .Where(point => point.Entity.Id == edge.Entity.PointFrom.Id)
                  .FirstOrDefault();
               if (currentPointFrom is null)
               {
                  throw new NullReferenceException(nameof(currentPointFrom));
               }

               var newEdge = Solver.CityMapGraph.GetEdge(currentPointFrom.Entity.Id, node.Entity.Id);
               if (newEdge is null)
               {
                  throw new NullReferenceException(nameof(newEdge));
               }

               // Tempo di percorrenza dell'arco in minuti.
               double tWalkMinutes = (newEdge.Weight() / averageSpeedWalk) / 60.0;
               TimeSpan tEdgeWalk = TimeSpan.FromMinutes(tWalkMinutes);

               if (tEdgeWalk < tWalk)
               {
                  InterestPointWorker currentPointTo = _currentSolutionGraph.TourPoints
                     .Where(point => point.Entity.Id == edge.Entity.PointTo.Id)
                     .FirstOrDefault();
                  if (currentPointTo is null)
                  {
                     throw new NullReferenceException(nameof(currentPointTo));
                  }

                  int pointToScore = currentPointTo.Entity.Score.Value;
                  if (pointToScore < currentPointScore)
                  {
                     tourPointToRemove = currentPointTo;    // Nodo da rimuovere
                     currentPointScore = pointToScore;
                     predecessorPoint = currentPointFrom;   // Nodo predecessore al nodo da rimuovere
                     ingoingRoute = edge;                   // Arco incidente sul nodo da rimuovere
                  }
               }
            }

            if (tourPointToRemove is null)
            {
               continue;
            }

            RouteWorker outgoingEdge = _currentSolutionGraph.Edges
               .Where(edge => edge.Entity.PointFrom.Id == tourPointToRemove.Entity.Id)
               .FirstOrDefault();
            if (outgoingEdge is null)
            {
               throw new NullReferenceException(nameof(outgoingEdge));
            }

            InterestPointWorker successorPoint = _currentSolutionGraph.TourPoints
               .Where(point => point.Entity.Id == outgoingEdge.Entity.PointTo.Id)
               .FirstOrDefault();
            if (successorPoint is null)
            {
               throw new NullReferenceException(nameof(successorPoint));
            }

            /*
             * NOTA
             * Prima di aggiungere il nuovo nodo al Tour, devo ulteriormente verificare che anche l'arco 
             * che va dal nuovo nodo da aggiungere al tour verso il nodo successore del nodo da rimuovere 
             * dal tour abbia un tempo di percorrenza inferiore rispetto al tempo di percorrenza dell'arco 
             * che va dal nodo da rimuovere verso il suo nodo successore?
             */

            // Primo taglio: Apertura del tour.
            _currentSolutionGraph.RemoveEdge(predecessorPoint.Entity.Id, tourPointToRemove.Entity.Id);
            _currentSolutionGraph.RemoveEdge(tourPointToRemove.Entity.Id, successorPoint.Entity.Id);
            _currentSolutionGraph.RemoveNode(tourPointToRemove.Entity.Id);

            // Unione: chiusura del tour con il nuovo nodo.
            _currentSolutionGraph.AddNode(node.Entity.Id, node);
            _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, predecessorPoint.Entity.Id, node.Entity.Id);
            _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, node.Entity.Id, successorPoint.Entity.Id);

            _currentSolution = new TOSolution()
            {
               SolutionGraph = _currentSolutionGraph
            };
            tourUpdated = true;
            Solver.EnqueueSolution(_currentSolution);
            await Solver.AlgorithmTasks[_currentSolution.Id];

            if (!_currentSolution.IsValid)
            {
               // Ripristino dello stato precedente del tour.
               _currentSolutionGraph.RemoveEdge(predecessorPoint.Entity.Id, node.Entity.Id);
               _currentSolutionGraph.RemoveEdge(node.Entity.Id, successorPoint.Entity.Id);
               _currentSolutionGraph.RemoveNode(node.Entity.Id);

               _currentSolutionGraph.AddNode(tourPointToRemove.Entity.Id, tourPointToRemove);
               _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, predecessorPoint.Entity.Id, tourPointToRemove.Entity.Id);
               _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, tourPointToRemove.Entity.Id, successorPoint.Entity.Id);

               tourUpdated = false;
            }
            await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);
         }
      }

      private async Task TryUpdateTour_v2()
      {
         double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed;
         var removalEdgesCandidates = CalculateMaximumEdgesTimeWalk(averageSpeedWalk);
         if (!removalEdgesCandidates.Any())
         {
            return;
         }

         AddPointsNotInCurrentTour();
         _processingNodes.OrderByDescending(point => point.Entity.Score.Value);

         InterestPointWorker tourPointToRemove = default;
         InterestPointWorker predecessorPoint = default;
         RouteWorker ingoingRoute = default;
         int currentPointScore = int.MaxValue;
         bool tourUpdated = default;

         while (_processingNodes.Any())
         {
            if (tourUpdated)
            {
               break;
            }
            InterestPointWorker candidateNode = _processingNodes.Dequeue();

            foreach (var (edge, tWalk) in removalEdgesCandidates)
            {
               InterestPointWorker currentPointFrom = _currentSolutionGraph.TourPoints
                  .Where(point => point.Entity.Id == edge.Entity.PointFrom.Id)
                  .FirstOrDefault();
               if (currentPointFrom is null)
               {
                  throw new NullReferenceException(nameof(currentPointFrom));
               }

               var newEdge = Solver.CityMapGraph.GetEdge(currentPointFrom.Entity.Id, candidateNode.Entity.Id);
               if (newEdge is null)
               {
                  throw new NullReferenceException(nameof(newEdge));
               }

               // Tempo di percorrenza dell'arco in minuti.
               double tWalkMinutes = (newEdge.Weight() / averageSpeedWalk) / 60.0;
               TimeSpan tEdgeWalk = TimeSpan.FromMinutes(tWalkMinutes);

               if (tEdgeWalk < tWalk)
               {
                  InterestPointWorker currentPointTo = _currentSolutionGraph.TourPoints
                     .Where(point => point.Entity.Id == edge.Entity.PointTo.Id)
                     .FirstOrDefault();
                  if (currentPointTo is null)
                  {
                     throw new NullReferenceException(nameof(currentPointTo));
                  }

                  int pointToScore = currentPointTo.Entity.Score.Value;
                  if (pointToScore < currentPointScore)
                  {
                     tourPointToRemove = currentPointTo;    // Nodo da rimuovere
                     currentPointScore = pointToScore;
                     predecessorPoint = currentPointFrom;   // Nodo predecessore al nodo da rimuovere
                     ingoingRoute = edge;                   // Arco incidente sul nodo da rimuovere
                  }
               }
            }

            if (tourPointToRemove is null)
            {
               continue;
            }

            RouteWorker outgoingEdge = _currentSolutionGraph.Edges
               .Where(edge => edge.Entity.PointFrom.Id == tourPointToRemove.Entity.Id)
               .FirstOrDefault();
            if (outgoingEdge is null)
            {
               throw new NullReferenceException(nameof(outgoingEdge));
            }

            InterestPointWorker successorPoint = _currentSolutionGraph.TourPoints
               .Where(point => point.Entity.Id == outgoingEdge.Entity.PointTo.Id)
               .FirstOrDefault();
            if (successorPoint is null)
            {
               throw new NullReferenceException(nameof(successorPoint));
            }

            /*
             * NOTA
             * Prima di aggiungere il nuovo nodo al Tour, devo ulteriormente verificare che anche l'arco 
             * che va dal nuovo nodo da aggiungere al tour verso il nodo successore del nodo da rimuovere 
             * dal tour abbia un tempo di percorrenza inferiore rispetto al tempo di percorrenza dell'arco 
             * che va dal nodo da rimuovere verso il suo nodo successore?
             */

            // Primo taglio: Apertura del tour.
            _currentSolutionGraph.RemoveEdge(predecessorPoint.Entity.Id, tourPointToRemove.Entity.Id);
            _currentSolutionGraph.RemoveEdge(tourPointToRemove.Entity.Id, successorPoint.Entity.Id);
            _currentSolutionGraph.RemoveNode(tourPointToRemove.Entity.Id);

            // Unione: chiusura del tour con il nuovo nodo.
            _currentSolutionGraph.AddNode(candidateNode.Entity.Id, candidateNode);
            _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, predecessorPoint.Entity.Id, candidateNode.Entity.Id);
            _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, candidateNode.Entity.Id, successorPoint.Entity.Id);

            _currentSolution = new TOSolution()
            {
               SolutionGraph = _currentSolutionGraph
            };
            tourUpdated = true;
            Solver.EnqueueSolution(_currentSolution);
            await Solver.AlgorithmTasks[_currentSolution.Id];

            if (!_currentSolution.IsValid)
            {
               // Ripristino dello stato precedente del tour.
               _currentSolutionGraph.RemoveEdge(predecessorPoint.Entity.Id, candidateNode.Entity.Id);
               _currentSolutionGraph.RemoveEdge(candidateNode.Entity.Id, successorPoint.Entity.Id);
               _currentSolutionGraph.RemoveNode(candidateNode.Entity.Id);

               _currentSolutionGraph.AddNode(tourPointToRemove.Entity.Id, tourPointToRemove);
               _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, predecessorPoint.Entity.Id, tourPointToRemove.Entity.Id);
               _currentSolutionGraph.AddRouteFromGraph(Solver.CityMapGraph, tourPointToRemove.Entity.Id, successorPoint.Entity.Id);

               tourUpdated = false;
            }
            await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);
         }

         _isTourUpdated = tourUpdated;
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
         _timeWalkThreshold = new TimeSpan(0, 30, 0);
         _tMaxThreshold = Solver.CurrentStage.Flow.HndTmaxThreshold;
         _tMax = Solver.WorkingConfiguration.ArrivalTime.Add(Solver.WorkingConfiguration.TourDuration);
         _startPOI = _currentSolutionGraph.GetStartPoint();
         _endPOI = _currentSolutionGraph.GetEndPoint();

         if (_startPOI is null || _endPOI is null)
         {
            throw new NullReferenceException();
         }

         AddPointsNotInCurrentTour();
         _processingNodes.OrderByDescending(point => point.Entity.Score.Value);
         _addedNodesCount = _processingNodes.Count;
         _isTourUpdated = false;
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
         await Solver.AlgorithmTasks[_currentSolution.Id];

         if (!_currentSolution.IsValid)
         {
            UndoAdditionPoint(point);
            _addedNodesCount--;
         }
         await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);
      }

      internal override void OnError(Exception exception)
      {
         _processingNodes.Clear();
         _processingNodes = null;
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();

         if (_addedNodesCount == 0)
         {
            Task updateTourTask = Task.Run(() => TryUpdateTour_v2());

            try
            {
               updateTourTask.Wait();
            }
            catch (AggregateException ae)
            {
               OnError(ae.InnerException);
            }
         }

         _processingNodes.Clear();
         _processingNodes = null;

         //bool isBetterThanCurrentBestSolution =
         //   Solver.Problem.CompareSolutionsCost(_currentSolution.Cost, Solver.BestSolution.Cost);
         //if (isBetterThanCurrentBestSolution)
         //{
         //   Solver.BestSolution = _currentSolution;
         //}

         if (_isTourUpdated)
         {
            Solver.BestSolution = _currentSolution;
            Task algorithmTask = Task.Run(() => new HybridNearestDistanceInsertion().Start());

            try
            {
               algorithmTask.Wait();
            }
            catch (AggregateException ae)
            {
               OnError(ae.InnerException);
            }
         }
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         InterestPointWorker endPOI = _currentSolutionGraph.GetEndPoint();
         TimeSpan availableTime = _tMax.Subtract(endPOI.TotalTime);

         bool isGreaterThanTmaxThreshold = availableTime > _tMaxThreshold;
         bool shouldStop = isGreaterThanTmaxThreshold ||
            _processingNodes.Count() == 0 || Status == AlgorithmStatus.Error;

         return shouldStop;
      }
      #endregion
   }
}