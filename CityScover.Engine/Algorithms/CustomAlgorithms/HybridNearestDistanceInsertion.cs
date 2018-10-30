//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 29/10/2018
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
      private HybridNearestDistanceUpdate _updateAlgorithm;
      private int _addedNodesCount;
      #endregion

      #region Protected members
      protected InterestPointWorker _startPOI;
      protected InterestPointWorker _endPOI;
      protected DateTime _tMax;
      protected TimeSpan _tMaxThreshold;
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

      #region Private Methods
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
         // Insieme delle tuple contenenti le informazioni: (Tratta, Tempo di percorrenza).
         Collection<(RouteWorker, TimeSpan)> removalCandidates = new Collection<(RouteWorker, TimeSpan)>();
         Collection<InterestPointWorker> candidatesToAdd = new Collection<InterestPointWorker>();

         AddPointsNotInCurrentTour();

         foreach (var route in _currentSolutionGraph.Edges)
         {
            /* 
             * TODO
             * 1. Calcolare il tempo di percorrenza a piedi del tratto corrente in minuti.
             * 2. Verificare se tale tempo supera una soglia fissata a priori.
             * 3. Se la condizione al punto 2 è verificata, aggiungere tale arco tra quelli candidati da rimuovere.
             */
         }

         foreach (var edge in removalCandidates)
         {
            /*
             * TODO
             * 
             * Per ogni nodo appartenente alla coda dei nodi non appartenenti al tour corrente (_processingNodes)
             * effettuare i seguenti passaggi:
             * 
             * 1. Dal grafo completo calcolare il tempo di percorrenza dell'arco che va dal nodo PointFrom dell'arco corrente (edge),
             * ad un nodo di quelli appartenenti alla coda _processingNodes (ovvero i nodi non appartenenti al tour corrente).
             * 
             * 2. Se il tempo di percorrenza appena calcolato relativo al nuovo arco, è inferiore al tempo di percorrenza dell'arco corrente,
             * rimuovere dal tour il nodo PointTo dell'arco corrente e l'arco associato tra PointFrom e PointTo dal tour.
             * 
             * 3. Aggiungere il nuovo nodo della coda _processingNodes che si sta valutando al tour.
             * 4. Aggiungere il nuovo arco al tour, che va dal nodo PointFrom dell'arco corrente al nuovo nodo appena aggiunto al Tour.
             * 5. Ripetere il procedimento per il nodo successivo di _processingNodes.
             * 
             * ******************************************************************************************************
             * NOTA:
             * Invece che aggiungere direttamente il nuovo nodo della coda al tour, 
             * usare una lista di potenziali nodi candidati da aggiungere al tour, i cui tempi 
             * di percorrenza dei relativi archi precedentemente calcolati (vedi punto 1), 
             * sono sicuramente inferiori rispetto al tempo di percorrenza dell'arco corrente che si sta esaminando.
             * ******************************************************************************************************
             * Al termine della scansione di tutti i nodi della coda _processingNodes, selezionare dall'insieme dei candidati da aggiungere
             * la coppia (nodo-arco) il cui score del nodo è il maggiore fra tutti.
             */
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
         _tMaxThreshold = Solver.CurrentStage.Flow.HndTmaxThreshold;
         _tMax = Solver.WorkingConfiguration.ArrivalTime.Add(Solver.WorkingConfiguration.TourDuration);
         _startPOI = _currentSolutionGraph.GetStartPoint();
         _endPOI = _currentSolutionGraph.GetEndPoint();

         if (_startPOI is null || _endPOI is null)
         {
            throw new NullReferenceException();
         }

         AddPointsNotInCurrentTour();
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
            Task.WaitAll(updateTourTask);
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