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

using CityScover.Engine.Workers;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CityScover.Engine.Algorithms.CustomAlgorithms;

namespace CityScover.Engine.Algorithms.Greedy
{
   internal abstract class GreedyTemplate : Algorithm
   {
      private DateTime _timeSpent;
      private bool _canDoImprovements;

      #region Protected fields
      protected CityMapGraph CityMapClone;
      protected InterestPointWorker StartingPoint;
      protected CityMapGraph Tour;
      protected ICollection<ToSolution> SolutionsHistory;
      protected Queue<int> ProcessingNodes;
      #endregion

      #region Constructors
      internal GreedyTemplate(AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      private async Task RunImprovementAlgorithm()
      {
         var childrenFlows = Solver.CurrentStage.Flow.ChildrenFlows;

         foreach (var algorithm in Solver.GetImprovementAlgorithms(childrenFlows))
         {
            Algorithm improvementAlgorithm = algorithm;
            algorithm.Provider = Provider;

            if (algorithm is HybridCustomUpdate hcu)
            {
               improvementAlgorithm = hcu;
               hcu.CanContinueToRelaxConstraints = true;
            }

            Solver.CurrentAlgorithm = algorithm.Type;
            await Task.Run(improvementAlgorithm.Start);
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

            var adjNodeScore = adjNode.Entity.Score.Value;
            if (adjNodeScore > bestScore)
            {
               bestScore = adjNodeScore;
               candidateNode = adjNode;
            }
            else if (adjNodeScore == bestScore)
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

         Console.ForegroundColor = ConsoleColor.Green;
         SendMessage(MessageCode.GreedyStart, Type);
         Console.ForegroundColor = ConsoleColor.Gray;

         Tour = new CityMapGraph();
         ProcessingNodes = new Queue<int>();
         SolutionsHistory = new Collection<ToSolution>();
         CityMapClone = Solver.CityMapGraph.DeepCopy();
         _canDoImprovements = default;
         int maxNodesToAdd = default;

         if (Parameters.ContainsKey(ParameterCodes.CanDoImprovements))
         {
            _canDoImprovements = Parameters[ParameterCodes.CanDoImprovements];
            if (_canDoImprovements)
            {
               if (Parameters.ContainsKey(ParameterCodes.GreedyMaxNodesToAdd))
               {
                  maxNodesToAdd = Parameters[ParameterCodes.GreedyMaxNodesToAdd];
               }
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

         _timeSpent = DateTime.Now;
         StartingPoint.IsVisited = true;
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         var validSolutions = SolutionsHistory.Where(solution => solution.IsValid);

         if (validSolutions.Any())
         {
            Solver.BestSolution = validSolutions.MaxBy(solution => solution.Cost);
            return;
         }
         Solver.BestSolution = SolutionsHistory.MaxBy(solution => solution.Cost);
      }

      internal override void OnTerminated()
      {
         CityMapClone = null;
         SendMessage(ToSolution.SolutionCollectionToString(SolutionsHistory));
         Task.WaitAll(Solver.AlgorithmTasks.Values.ToArray());

         Console.ForegroundColor = ConsoleColor.Green;
         SendMessage(MessageCode.GreedyStop);
         Console.ForegroundColor = ConsoleColor.Gray;

         base.OnTerminated();

         if (!_canDoImprovements)
         {
            return;
         }

         Task improvementTask = RunImprovementAlgorithm();
         try
         {
            improvementTask.Wait();
         }
         catch (AggregateException ae)
         {
            OnError(ae.InnerException);
         }
      }

      internal override bool StopConditions()
      {
         return !ProcessingNodes.Any() || base.StopConditions();
      }
      #endregion
   }
}