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
      protected double AverageSpeedWalk;
      protected CityMapGraph CityMapClone;
      protected DateTime TimeSpent;
      protected InterestPointWorker StartingPoint;
      protected CityMapGraph Tour;
      protected ICollection<TOSolution> SolutionsHistory;
      protected Queue<int> ProcessingNodes;
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

         foreach (var child in childrenAlgorithms)
         {
            var algorithm = Solver.GetAlgorithm(child.CurrentAlgorithm);
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

         var adjPoiIds = CityMapClone.GetAdjacentNodes(interestPoint.Entity.Id);
         adjPoiIds.ToList().ForEach(adjPoiId =>
         {
            var adjNode = CityMapClone[adjPoiId];
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
               candidateNode = CityMapClone[pointId];
            }
         });

         return candidateNode;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         AverageSpeedWalk = Solver.WorkingConfiguration.WalkingSpeed;
         Tour = new CityMapGraph();
         ProcessingNodes = new Queue<int>();
         SolutionsHistory = new Collection<TOSolution>();
         CityMapClone = Solver.CityMapGraph.DeepCopy();
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
            .ForEach(nodeId => ProcessingNodes.Enqueue(nodeId));

         if (maxNodesToAdd != default)
         {
            ProcessingNodes.Clear();
            Solver.CityMapGraph.TourPoints
               .Take(maxNodesToAdd)
               .Select(node => node.Entity.Id).ToList()
               .ForEach(nodeId => ProcessingNodes.Enqueue(nodeId));
         }

         StartingPoint = CityMapClone.GetStartPoint();
         if (StartingPoint is null)
         {
            throw new OperationCanceledException(
               $"{nameof(StartingPoint)} in {nameof(NearestNeighbor)}");
         }

         TimeSpent = DateTime.Now;
         StartingPoint.IsVisited = true;
      }

      internal override void OnError(Exception exception)
      {
         CurrentStep = default;
         TOSolution lastProducedSolution = SolutionsHistory.Last();
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = SolutionsHistory.Last();
      }

      internal override void OnTerminated()
      {
         CityMapClone = null;
         TOSolution bestProducedSolution = SolutionsHistory.Last();

         SendMessage(TOSolution.SolutionCollectionToString(SolutionsHistory));

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
         return !ProcessingNodes.Any() || base.StopConditions();
      }
      #endregion
   }
}