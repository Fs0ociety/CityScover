//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 22/11/2018
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
      private TOSolution _currentBestSolution;
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
         Type = AlgorithmType.TabuSearch;
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
         _neighborhood.Type = _neighborhood.NeighborhoodWorker.Type;
         Algorithm algorithm = Solver.GetAlgorithm(flow.CurrentAlgorithm, _neighborhood.NeighborhoodWorker);
         
         if (algorithm is LocalSearchTemplate ls)
         {
            algorithm.Parameters = flow.AlgorithmParameters;
            ls.CurrentBestSolution = _currentBestSolution;
            _innerAlgorithm = ls;
         }
         else
         {
            throw new NullReferenceException(nameof(algorithm));
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

      private async Task RunLocalSearch()
      {
         Solver.CurrentAlgorithm = _innerAlgorithm.Type;
         await Task.Run(() => _innerAlgorithm.Start());
         Solver.CurrentAlgorithm = Type;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _maxIterations = Solver.CurrentStage.Flow.RunningCount;
         _maxDeadlockIterations = Parameters[ParameterCodes.TABUmaxDeadlockIterations];
         int tabuTenureFactor = Parameters[ParameterCodes.TABUtenureFactor];

         if (tabuTenureFactor == 0)
         {
            throw new InvalidOperationException($"Bad configuration format: " +
               $"{nameof(ParameterCodes.TABUtenureFactor)}.");
         }

         _tenure = CalculateTabuTenure(Solver.ProblemSize, tabuTenureFactor);
         _innerAlgorithm = GetLocalSearchAlgorithm();

         if (_innerAlgorithm is null)
         {
            throw new InvalidOperationException($"Bad configuration format: " +
               $"{nameof(Solver.WorkingConfiguration)}.");
         }

         _innerAlgorithm.AcceptImprovementsOnly = false;
         _innerAlgorithm.Provider = Provider;
         _currentBestSolution = Solver.BestSolution;
      }

      internal override async Task PerformStep()
      {
         _neighborhood.TabuList.ToList().ForEach(move => move.Expiration++);
         await RunLocalSearch();

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
         Solver.BestSolution = _currentBestSolution;
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         bool shouldStop = _currentIteration == _maxIterations
            || base.StopConditions();

         if (_maxDeadlockIterations > 0)
         {
            shouldStop = shouldStop || _noImprovementsCount == _maxDeadlockIterations;
         }
         return shouldStop;
      }
      #endregion
   }
}