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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Greedy
{
   internal abstract class GreedyTemplate : Algorithm
   {
      #region Protected fields
      protected double _averageSpeedWalk;
      protected CityMapGraph _cityMapClone;
      protected DateTime _timeSpent;
      protected ICollection<TOSolution> _solutions;
      protected CityMapGraph _tour;
      protected InterestPointWorker _startingPoint;
      #endregion

      #region Constructors
      internal GreedyTemplate() 
         : this(null)
      {
      }

      internal GreedyTemplate(AlgorithmTracker provider) 
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      private void SetRandomCandidateId(InterestPointWorker candidateNode, InterestPointWorker adjNode, out int id)
      {
         if (candidateNode is null)
         {
            id = adjNode.Entity.Id;
         }
         else
         {
            id = (new Random().Next(2) != 0)
               ? candidateNode.Entity.Id
               : adjNode.Entity.Id;
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
               SetRandomCandidateId(candidateNode, adjNode, out int pointId);
               candidateNode = _cityMapClone[pointId];
            }
         });
         // Caso particolare (gestito solo per irrobustire il codice): se ho 2 nodi del grafo, e
         // il secondo è già stato visitato, io ritorno null come candidateNode.
         return candidateNode;
      }

      protected DateTime GetTotalTime()
      {
         DateTime timeSpent = Solver.Instance.WorkingConfiguration.ArrivalTime;
         InterestPointWorker startPOI = _tour.GetStartPoint();
         InterestPointWorker endPOI = _tour.GetEndPoint();
         TimeSpan endPOITotalTimeDuration = endPOI.TotalTime.Subtract(timeSpent);
         timeSpent = timeSpent.Add(endPOITotalTimeDuration);

         RouteWorker returnEdge = _cityMapClone.GetEdge(endPOI.Entity.Id, startPOI.Entity.Id);
         if (returnEdge == null)
         {
            throw new NullReferenceException(nameof(returnEdge));
         }

         double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed;
         TimeSpan timeReturn = TimeSpan.FromSeconds(returnEdge.Weight() / averageSpeedWalk);
         timeSpent = timeSpent.Add(timeReturn);
         return timeSpent;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

         _averageSpeedWalk = Solver.WorkingConfiguration.WalkingSpeed;
         _solutions = new Collection<TOSolution>();
         _tour = new CityMapGraph();
         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         _timeSpent = DateTime.Now;
         _startingPoint = _cityMapClone.GetStartPoint();

         if (_startingPoint == null)
         {
            throw new OperationCanceledException(
               $"{nameof(_startingPoint)} in {nameof(NearestNeighbor)}");
         }

         _startingPoint.IsVisited = true;
      }

      internal override void OnError(Exception exception)
      {
         CurrentStep = default;
         TOSolution lastProducedSolution = _solutions.Last();
         Result resultError =
            new Result(lastProducedSolution, CurrentAlgorithm, _timeSpent, Result.Validity.Invalid);
         resultError.ResultFamily = AlgorithmFamily.Greedy;
         Solver.Results.Add(resultError);
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _solutions.Last();
      }

      internal override void OnTerminated()
      {
         _cityMapClone = null;
         TOSolution bestProducedSolution = _solutions.Last();
         Result validResult =
            new Result(bestProducedSolution, CurrentAlgorithm, _timeSpent, Result.Validity.Valid);
         validResult.ResultFamily = AlgorithmFamily.Greedy;
         Solver.Results.Add(validResult);
         Task.WaitAll(Solver.AlgorithmTasks.Values.ToArray());
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return Status == AlgorithmStatus.Error;
      }
      #endregion
   }
}