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
      //private HybridNearestDistanceUpdate _updateAlgorithm;
      private int _addedNodesCount;
      #endregion

      #region Protected members
      protected InterestPointWorker _startPOI;
      protected InterestPointWorker _endPOI;
      protected DateTime _tMax;
      protected TimeSpan _tMaxThreshold;
      protected TimeSpan _timeWalkThreshold;
      protected TOSolution _currentSolution;
      protected CityMapGraph _currentSolutionGraph;
      protected Queue<InterestPointWorker> _processingNodes;
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

      private async Task TryUpdateTour()
      {
         double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed;

         // Insieme delle tuple contenenti le informazioni: (Tratta, Tempo di percorrenza).
         Collection<(RouteWorker edge, TimeSpan tWalk)> removalEdgesCandidates =
            new Collection<(RouteWorker, TimeSpan)>();

         // Insieme contenente i nodi appartenenti al tour corrente e candidati alla rimozione.
         Collection<InterestPointWorker> removalNodesCandidates =
            new Collection<InterestPointWorker>();

         foreach (var route in _currentSolutionGraph.Edges)
         {
            TimeSpan timeRouteWalk = TimeSpan.FromMinutes((route.Weight() / averageSpeedWalk) / 60.0);

            if (timeRouteWalk > _timeWalkThreshold)
            {
               var removalEdgeCandidate = (route, timeRouteWalk);
               removalEdgesCandidates.Add(removalEdgeCandidate);
            }
         }

         if (!removalEdgesCandidates.Any())
         {
            return;
         }

         AddPointsNotInCurrentTour();
         _processingNodes.OrderByDescending(point => point.Entity.Score.Value);

         foreach (var node in _processingNodes)
         {
            int currentPointScore = int.MaxValue;
            InterestPointWorker currentPointFrom = default;
            InterestPointWorker tourPointToRemove = default;
            RouteWorker ingoingEdge = default;

            foreach (var (edge, tWalk) in removalEdgesCandidates)
            {
               currentPointFrom = _currentSolutionGraph.TourPoints
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

               TimeSpan tEdgeWalk = TimeSpan.FromMinutes((newEdge.Weight() / averageSpeedWalk) / 60.0);

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
                     tourPointToRemove = currentPointTo;
                     currentPointScore = pointToScore;
                     ingoingEdge = edge;
                  }

                  //tourPointToRemove = _currentSolutionGraph.TourPoints
                  //   .Where(point => point.Entity.Id == edge.Entity.PointTo.Id)
                  //   .FirstOrDefault();

                  //if (tourPointToRemove is null)
                  //{
                  //   throw new NullReferenceException(nameof(tourPointToRemove));
                  //}
                  //removalNodesCandidates.Add(tourPointToRemove);
               }
            }

            if (tourPointToRemove is null)
            {
               continue;
            }

            //if (removalNodesCandidates.Any())
            //{
            //   tourPointToRemove = removalNodesCandidates
            //      .OrderBy(candidate => candidate.Entity.Score.Value)
            //      .FirstOrDefault();

            //   removalNodesCandidates.Clear();

            //RouteWorker ingoingEdge = removalEdgesCandidates
            //   .Where(item => item.edge.Entity.PointTo.Id == tourPointToRemove.Entity.Id)
            //   .Select(item => item.edge).FirstOrDefault();

            RouteWorker outgoingEdge = _currentSolutionGraph.Edges
               .Where(edge => edge.Entity.PointFrom.Id == tourPointToRemove.Entity.Id)
               .FirstOrDefault();

            InterestPointWorker sourcePoint = _currentSolutionGraph.TourPoints
               .Where(point => point.Entity.Id == ingoingEdge.Entity.PointFrom.Id)
               .FirstOrDefault();

            InterestPointWorker destPoint = _currentSolutionGraph.TourPoints
               .Where(point => point.Entity.Id == outgoingEdge.Entity.PointTo.Id)
               .FirstOrDefault();

            // TODO
            // 1. rimuovere i due archi dal tour tenendo da parte i rispettivi nodi PointFrom e PointTo da usare per ricollegare il grafo.
            // 2. rimuovere il nodo tourPointToRemove dal tour
            // 3. Aggiungere node al tour
            // 4. Collegare i rispettivi archi
            // 5. Costruisci la nuova Solution e inviala in coda da validare
            // 6. Se è valida riparti con la nuova soluzione e prova ad aggiungere il nodo successivo prelevato dalla coda
            // 7. Se non è valida, ripristina la situazione precedente con il nodo precedente rimosso dal tour.
            // 8. Riparto dalla situazione con un nuovo candidato della coda dei nodi non appartenenti al tour. (_processingNodes)

            await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);
            //}
         }
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
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();

         if (_addedNodesCount == 0)
         {
            Task updateTourTask = Task.Run(() => TryUpdateTour());

            try
            {
               updateTourTask.Wait();
            }
            catch (AggregateException ae)
            {
               OnError(ae.InnerException);
            }

            //_updateAlgorithm = new HybridNearestDistanceUpdate(Provider);
            //Task algorithmTask = Task.Run(() => _updateAlgorithm.Start());
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