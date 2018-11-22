﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 20/11/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Greedy
{
   internal abstract class GreedyTemplate : Algorithm
   {
      private bool _canDoImprovements;

      #region Protected fields
      protected double _averageSpeedWalk;
      protected CityMapGraph _cityMapClone;
      protected DateTime _timeSpent;
      protected InterestPointWorker _startingPoint;
      protected CityMapGraph _tour;
      protected ICollection<TOSolution> _solutionsHistory;
      protected Queue<int> _processingNodes;
      #endregion

      #region Constructors
      internal GreedyTemplate()
         : this(provider: null)
      {
      }

      internal GreedyTemplate(AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      private IEnumerable<Algorithm> GetImprovementAlgorithms()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms is null)
         {
            yield return null;
         }

         Algorithm algorithm = default;
         foreach (var child in childrenAlgorithms)
         {
            algorithm = Solver.GetAlgorithm(child.CurrentAlgorithm);
            if (algorithm is null)
            {
               throw new NullReferenceException(nameof(algorithm));
            }

            algorithm.Parameters = child.AlgorithmParameters;
            algorithm.Provider = Provider;

            yield return algorithm;
         }
      }

      private async Task RunImprovementAlgorithms()
      {
         foreach (var algorithm in GetImprovementAlgorithms())
         {
            if (algorithm is null)
            {
               throw new InvalidOperationException($"Bad configuration format: " +
                  $"{nameof(Solver.WorkingConfiguration)}.");
            }

            Solver.CurrentAlgorithm = algorithm.Type;
            await Task.Run(() => algorithm.Start());
            Solver.CurrentAlgorithm = Type;
         }
      }
      #endregion

      #region Protected methods
      /// <summary>
      /// This implementation is the classic GetClosestNeighborByScore.
      /// It returns the best candidate node near to point of interest passed as argument.
      /// </summary>
      /// <param name="interestPoint">Point of interest</param>
      /// <returns></returns>
      protected virtual InterestPointWorker GetBestNeighbor(InterestPointWorker interestPoint)
      {
         int bestScore = default;
         InterestPointWorker candidateNode = default;

         var adjPOIIds = _cityMapClone.GetAdjacentNodes(interestPoint.Entity.Id);
         adjPOIIds.ToList().ForEach(adjPOIId =>
         {
            var adjNode = _cityMapClone[adjPOIId];
            if (adjNode.IsVisited)
            {
               return;
            }

            var deltaScore = Math.Abs(adjNode.Entity.Score.Value - interestPoint.Entity.Score.Value);
            if (deltaScore > bestScore)
            {
               bestScore = deltaScore;
               candidateNode = adjNode;
            }
            else if (deltaScore == bestScore)
            {
               CityMapGraph.SetRandomCandidateId(candidateNode, adjNode, out int pointId);
               candidateNode = _cityMapClone[pointId];
            }
         });

         return candidateNode;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _averageSpeedWalk = Solver.WorkingConfiguration.WalkingSpeed;
         _tour = new CityMapGraph();
         _processingNodes = new Queue<int>();
         _solutionsHistory = new Collection<TOSolution>();
         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         _canDoImprovements = default;
         int maxNodesToAdd = default;

         if (Parameters.ContainsKey(ParameterCodes.CanDoImprovements))
         {
            _canDoImprovements = Parameters[ParameterCodes.CanDoImprovements];
            if (_canDoImprovements)
            {
               maxNodesToAdd = Parameters[ParameterCodes.GREEDYmaxNodesToAdd];
            }
         }

         Solver.CityMapGraph.TourPoints
            .Select(node => node.Entity.Id)
            .ToList()
            .ForEach(nodeId => _processingNodes.Enqueue(nodeId));

         if (maxNodesToAdd != default)
         {
            _processingNodes.Clear();
            Solver.CityMapGraph.TourPoints
               .Take(maxNodesToAdd)
               .Select(node => node.Entity.Id).ToList()
               .ForEach(nodeId => _processingNodes.Enqueue(nodeId));
         }

         _startingPoint = _cityMapClone.GetStartPoint();
         if (_startingPoint is null)
         {
            throw new OperationCanceledException(
               $"{nameof(_startingPoint)} in {nameof(NearestNeighbor)}");
         }

         _timeSpent = DateTime.Now;
         _startingPoint.IsVisited = true;
      }

      internal override void OnError(Exception exception)
      {
         CurrentStep = default;
         TOSolution lastProducedSolution = _solutionsHistory.Last();
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _solutionsHistory.Last();
      }

      internal override void OnTerminated()
      {
         _cityMapClone = null;
         TOSolution bestProducedSolution = _solutionsHistory.Last();

         SendMessage(TOSolution.SolutionCollectionToString(_solutionsHistory));

         Task.WaitAll(Solver.AlgorithmTasks.Values.ToArray());
         SendMessage(MessageCode.GreedyFinish);
         base.OnTerminated();

         if (_canDoImprovements)
         {
            Task improvementTask = RunImprovementAlgorithms();
            try
            {
               improvementTask.Wait();
            }
            catch (AggregateException ae)
            {
               OnError(ae.InnerException);
            }
         }
      }

      internal override bool StopConditions()
      {
         return !_processingNodes.Any() || base.StopConditions();
      }
      #endregion
   }
}