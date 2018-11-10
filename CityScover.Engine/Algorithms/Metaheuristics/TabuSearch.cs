//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/11/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal class TabuSearch : Algorithm
   {
      #region Private fields
      private LocalSearchTemplate _innerAlgorithm;
      private TabuSearchNeighborhood _neighborhood;
      private TOSolution _bestSolution;
      private int _tenure;
      private int _maxIterations;
      private int _maxDeadlockIterations;
      private int _noImprovementsCount;
      private int _currentIteration;
      #endregion

      #region Constructors
      internal TabuSearch()
         : this(null)
      {
      }

      internal TabuSearch(Neighborhood neighborhood)
         : this(neighborhood, null)
      {
      }

      internal TabuSearch(Neighborhood neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
         if (neighborhood is TabuSearchNeighborhood tbNeighborhood)
         {
            _neighborhood = tbNeighborhood;
         }
      }
      #endregion

      #region Private methods
      private int CalculateTabuTenure(int problemSize, int factor)
      {
         #region Code for debug...
         //int start = (problemSize + 4 - 1) / 4;
         //int end = (problemSize + 2 - 1) / 2;

         //_tenure = new Random().Next(2) == 0 ? start : end;
         //return _tenure;
         #endregion

         //return new Random().Next(2) == 0 ?
         //   (problemSize + 4 - 1) / 4 :
         //   (problemSize + 2 - 1) / 2;

         return (problemSize + factor - 1) / factor;
      }

      private LocalSearchTemplate GetLocalSearchAlgorithm()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms is null)
         {
            return null;
         }

         var flow = childrenAlgorithms.FirstOrDefault();
         if (flow is null)
         {
            return null;
         }

         _neighborhood.NeighborhoodWorker = NeighborhoodFactory.CreateNeighborhood(flow.CurrentAlgorithm);
         Algorithm algorithm = Solver.GetAlgorithm(flow.CurrentAlgorithm, _neighborhood);

         if (algorithm is LocalSearchTemplate ls)
         {
            _innerAlgorithm = ls;
            _maxIterations = flow.RunningCount;
            _maxDeadlockIterations = flow.MaximumDeadlockIterations;
         }

         return _innerAlgorithm;
      }

      private void AspirationCriteria()
      {
         // Using LINQ with generic Predicate<T> delegate to remove items in Tabu list.
         //_neighborhood.TabuList.ToList().RemoveAll(tabuMove => tabuMove.Expiration >= _tenure);

         // Using classic foreach, iterating over a copy of the Tabu list.
         foreach (var tabuMove in _neighborhood.TabuList.ToList())
         {
            if (tabuMove.Expiration >= _tenure)
            {
               _neighborhood.TabuList.Remove(tabuMove);
            }
         }
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         if (Solver.IsMonitoringEnabled)
         {
            SendMessage(MessageCode.StageStart, Solver.CurrentStage.Description);
         }

         var algorithmParams = Solver.CurrentStage.Flow.AlgorithmParameters;
         int tabuTenureFactor = algorithmParams[ParameterCodes.TabuTenureFactor];

         if (tabuTenureFactor == 0)
         {
            throw new InvalidOperationException($"Bad configuration format: " +
               $"{nameof(ParameterCodes.TabuTenureFactor)}.");
         }

         _tenure = CalculateTabuTenure(Solver.ProblemSize, tabuTenureFactor);
         _innerAlgorithm = GetLocalSearchAlgorithm();

         if (_innerAlgorithm is null)
         {
            throw new InvalidOperationException($"Bad configuration format: " +
               $"{nameof(Solver.WorkingConfiguration)}.");
         }

         //_neighborhood.TabuList = new List<TabuMove>();
         _innerAlgorithm.AcceptImprovementsOnly = false;
         _innerAlgorithm.Provider = Provider;
         _bestSolution = Solver.BestSolution;
      }

      internal override async Task PerformStep()
      {
         _neighborhood.TabuList.ToList().ForEach(move => move.Expiration++);
         await _innerAlgorithm.Start();

         bool isBetterThanPreviousBestSolution =
            Solver.Problem.CompareSolutionsCost(Solver.BestSolution.Cost, Solver.PreviousStageSolutionCost);

         if (!isBetterThanPreviousBestSolution)
         {
            _noImprovementsCount++;
         }

         AspirationCriteria();
         _currentIteration++;
      }

      internal override void OnError(Exception exception)
      {
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _bestSolution;
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         bool shouldStop = _currentIteration == _maxIterations
            || Status == AlgorithmStatus.Error;

         if (_maxDeadlockIterations > 0)
         {
            shouldStop = shouldStop || _noImprovementsCount == _maxDeadlockIterations;
         }
         return shouldStop;
      }
      #endregion
   }
}