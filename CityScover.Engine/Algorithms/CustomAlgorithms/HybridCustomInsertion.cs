//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 20/12/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.CustomAlgorithms
{
   internal class HybridCustomInsertion : Algorithm
   {

      #region Private fields

      #region Constants
      private const string TimeThresholdToTmaxConstraint =
         "TimeThresholdToTmax";
      #endregion

      private DateTime _tMax;
      private DateTime _tMaxThresholdTime;
      private TimeSpan _timeThresholdToTmax;
      private ToSolution _currentSolution;
      private int _addedNodesCount;
      #endregion

      #region Constructors
      internal HybridCustomInsertion() : this(provider: null)
      {
      }

      internal HybridCustomInsertion(AlgorithmTracker provider) : base(provider)
         => Type = AlgorithmType.HybridCustomInsertion;
      #endregion

      #region Private Protected fields
      private protected CityMapGraph Tour;
      private protected InterestPointWorker StartPoi;
      private protected InterestPointWorker EndPoi;
      private protected Queue<InterestPointWorker> ProcessingNodes;
      private protected ICollection<ToSolution> SolutionsHistory;
      #endregion

      #region Private protected methods
      private protected void UpdateSolver(ToSolution newSolution, ConsoleColor color)
      {
         var (previousSolutionId, previousSolutionCost) = (Solver.BestSolution.Id, Solver.BestSolution.Cost);
         Solver.BestSolution = newSolution;

         Console.ForegroundColor = color;
         SendMessage(MessageCode.HybridCustomInsertionFinalSolution,
            newSolution.Id, newSolution.Cost, previousSolutionId, previousSolutionCost);
         Console.ForegroundColor = ConsoleColor.Gray;
      }
      #endregion

      #region Internal properties
      internal ToSolution CurrentBestSolution { get; set; }
      #endregion

      #region Private methods
      private void AddPointsNotInTour()
      {
         var currentSolutionNodes = Tour.Nodes;
         var cityMapGraphNodes = Solver.CityMapGraph.TourPoints;

         var cityMapGraphNodeIds = cityMapGraphNodes.Select(point => point.Entity.Id);
         var currentSolutionNodeIds = currentSolutionNodes.Select(point => point.Entity.Id);
         var filteredNodeIds = cityMapGraphNodeIds.Except(currentSolutionNodeIds);

         var orderedFilteredNodes = cityMapGraphNodes
            .Where(point => filteredNodeIds.Any(nodeId => nodeId == point.Entity.Id))
            .OrderByDescending(point => point.Entity.Score.Value);

         orderedFilteredNodes.ToList().ForEach(node => ProcessingNodes.Enqueue(node));
      }

      private void AddNode(InterestPointWorker nodeToAdd, int fromNodeKey, int toNodeKey)
      {
         int nodeKeyToAdd = nodeToAdd.Entity.Id;

         Tour.RemoveEdge(fromNodeKey, toNodeKey);
         Tour.AddNode(nodeKeyToAdd, nodeToAdd);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, fromNodeKey, nodeKeyToAdd);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, nodeKeyToAdd, toNodeKey);
      }

      private void UndoNodeAdditon(int nodeKeyToRemove, int fromNodeKey, int toNodeKey)
      {
         Tour.RemoveEdge(nodeKeyToRemove, toNodeKey);
         Tour.RemoveEdge(fromNodeKey, nodeKeyToRemove);
         Tour.RemoveNode(nodeKeyToRemove);
         Tour.AddRouteFromGraph(Solver.CityMapGraph, fromNodeKey, toNodeKey);
      }

      private HybridCustomUpdate RunUpdateTour()
      {
         var updateAlgorithm = (HybridCustomUpdate)Solver.GetAlgorithm(
            AlgorithmType.HybridCustomUpdate);

         if (updateAlgorithm is null)
         {
            throw new NullReferenceException(nameof(updateAlgorithm));
         }

         updateAlgorithm.Provider = Provider;
         updateAlgorithm.Parameters = Parameters;

         Solver.CurrentAlgorithm = updateAlgorithm.Type;
         Task updateTask = Task.Run(updateAlgorithm.Start);
         try
         {
            updateTask.Wait();
         }
         catch (AggregateException ae)
         {
            updateAlgorithm = null;
            OnError(ae.InnerException);
         }
         finally
         {
            Solver.CurrentAlgorithm = Type;
         }

         return updateAlgorithm;
      }

      #region TODO: Remove method Restart()
      //private void Restart()
      //{
      //   Algorithm algorithm = Solver.GetAlgorithm(AlgorithmType.HybridCustomInsertion);

      //   if (algorithm is null)
      //   {
      //      throw new NullReferenceException(nameof(algorithm));
      //   }

      //   algorithm.Provider = Provider;
      //   algorithm.Parameters = Parameters;

      //   Task algorithmTask = Task.Run(() => algorithm.Start());
      //   try
      //   {
      //      algorithmTask.Wait();
      //   }
      //   catch (AggregateException ae)
      //   {
      //      OnError(ae.InnerException);
      //   }
      //}
      #endregion

      private bool IsTimeThresholdToTmaxSatisfied(ToSolution solution)
      {
         var endPoint = solution.SolutionGraph.GetEndPoint();
         bool isSatisfied = endPoint.TotalTime <= _tMaxThresholdTime;
         return isSatisfied;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         if (!Parameters.Any())
         {
            throw new KeyNotFoundException(nameof(Parameters));
         }

         base.OnInitializing();

         if (Solver.IsMonitoringEnabled && Type == AlgorithmType.HybridCustomInsertion)
         {
            if (!Parameters.ContainsKey(ParameterCodes.HciTimeThresholdToTmax))
            {
               throw new KeyNotFoundException(nameof(ParameterCodes.HciTimeThresholdToTmax));
            }

            Solver.Problem.Constraints.Add(new KeyValuePair<string, Func<ToSolution, bool>>(
                  TimeThresholdToTmaxConstraint, IsTimeThresholdToTmaxSatisfied));

            _timeThresholdToTmax = Parameters[ParameterCodes.HciTimeThresholdToTmax];
            _tMax = Solver.WorkingConfiguration.ArrivalTime.Add(Solver.WorkingConfiguration.TourDuration);
            _tMaxThresholdTime = _tMax - _timeThresholdToTmax;

            Console.ForegroundColor = ConsoleColor.Yellow;
            SendMessage(MessageCode.HybridCustomInsertionStart);
            Console.ForegroundColor = ConsoleColor.Gray;
         }

         if (CurrentBestSolution is null)
         {
            CurrentBestSolution = Solver.BestSolution;
         }
         Solver.PreviousStageSolutionCost = CurrentBestSolution.Cost;
         Tour = CurrentBestSolution.SolutionGraph.DeepCopy();
         StartPoi = Tour.GetStartPoint() ?? throw new NullReferenceException(nameof(StartPoi));
         EndPoi = Tour.GetEndPoint() ?? throw new NullReferenceException(nameof(EndPoi));
         SolutionsHistory = new Collection<ToSolution>();
         ProcessingNodes = new Queue<InterestPointWorker>();
         AddPointsNotInTour();
      }

      protected override async Task PerformStep()
      {
         InterestPointWorker point = ProcessingNodes.Dequeue();

         AddNode(point, EndPoi.Entity.Id, StartPoi.Entity.Id);
         var previousEndPoiKey = EndPoi.Entity.Id;
         EndPoi = Tour.GetEndPoint();
         _addedNodesCount++;

         _currentSolution = new ToSolution()
         {
            SolutionGraph = Tour.DeepCopy()
         };

         Solver.EnqueueSolution(_currentSolution);
         await Task.Delay(Utils.ValidationDelay).ConfigureAwait(false);
         await Solver.AlgorithmTasks[_currentSolution.Id];
         SendMessage(MessageCode.HybridCustomInsertionNewNodeAdded,
            point.Entity.Name);
         SolutionsHistory.Add(_currentSolution);

         if (!_currentSolution.IsValid)
         {
            UndoNodeAdditon(point.Entity.Id, previousEndPoiKey, StartPoi.Entity.Id);
            EndPoi = Tour.GetEndPoint();
            _addedNodesCount--;
            SendMessage(MessageCode.HybridCustomInsertionNewNodeRemoved, point.Entity.Name);
         }

         if (Solver.IsMonitoringEnabled)
         {
            Provider.NotifyObservers(_currentSolution);
         }
      }

      internal override void OnError(Exception exception)
      {
         ProcessingNodes?.Clear();
         ProcessingNodes = null;
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         /*
          * TODO
          * Add two methods on Solver: AddConstraint and RemoveConstraint.
          * These methods can be called from an Algorithm to customize his work.
          */

         Solver.Problem.Constraints.Remove(
            Solver.Problem.Constraints.FirstOrDefault(constraint =>
               constraint.Key.Equals(TimeThresholdToTmaxConstraint)));

         Console.ForegroundColor = ConsoleColor.Yellow;
         SendMessage(MessageCode.HybridCustomInsertionStopWithSolution, _currentSolution.Id, _currentSolution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;

         if (_addedNodesCount == 0)
         {
            var updateAlgorithm = RunUpdateTour();

            if (updateAlgorithm.TourUpdated)
            {
               Console.ForegroundColor = ConsoleColor.Yellow;
               SendMessage(MessageCode.HybridCustomInsertionTourUpdated);
               Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
               Console.ForegroundColor = ConsoleColor.Yellow;
               SendMessage(MessageCode.HybridCustomInsertionTourNotUpdated);
               Console.ForegroundColor = ConsoleColor.Gray;
            }
         }
         else
         {
            // Surely exists valid solutions here!
            var bestSolution = SolutionsHistory
               .Where(solution => solution.IsValid)
               .MaxBy(solution => solution.Cost);

            UpdateSolver(bestSolution, ConsoleColor.Yellow);

            // Remove only Tmax constraint from ConstraintsToRelax collection.
            Solver.ConstraintsToRelax.Remove(Utils.TMaxConstraint);
            SendMessage(ToSolution.SolutionCollectionToString(SolutionsHistory));
         }
      }

      internal override void OnTerminated()
      {
         SolutionsHistory?.Clear();
         ProcessingNodes?.Clear();
         SolutionsHistory = null;
         ProcessingNodes = null;
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return !ProcessingNodes.Any() || Status == AlgorithmStatus.Error;
      }
      #endregion
   }
}