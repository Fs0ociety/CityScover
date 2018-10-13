//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 13/10/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal class TabuSearch : Algorithm
   {
      #region Private fields
      private LocalSearch _innerAlgorithm;
      private TabuSearchNeighborhood _neighborhood;
      private TOSolution _bestSolution;
      private int _tenure;
      private int _maxIterations;
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
      private int CalculateTabuTenure(int problemSize)
      {
         #region Code for testing...
         //int start = (problemSize + 8 - 1) / 8;
         //int end = (problemSize + 4 - 1) / 4;

         //_tenure = new Random().Next(2) == 0 ? start : end;
         //return _tenure;
         #endregion

         return new Random().Next(2) == 0 ?
            (problemSize + 8 - 1) / 8 :
            (problemSize + 4 - 1) / 4;
      }

      private LocalSearch GetLocalSearchAlgorithm()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms == null)
         {
            return null;
         }

         var flow = childrenAlgorithms.FirstOrDefault();
         if (flow == null)
         {
            return null;
         }

         _neighborhood.NeighborhoodWorker = NeighborhoodFactory.CreateNeighborhood(flow.CurrentAlgorithm);
         Algorithm algorithm = Solver.GetAlgorithm(flow.CurrentAlgorithm, _neighborhood);

         if (algorithm is LocalSearch ls)
         {
            _innerAlgorithm = ls;
            _maxIterations = flow.RunningCount;
         }

         return _innerAlgorithm;
      }

      private void AspirationCriteria()
      {
         foreach (var tabuMove in _neighborhood.TabuList)
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
         _bestSolution = Solver.BestSolution;
         _tenure = CalculateTabuTenure(Solver.ProblemSize);
         _innerAlgorithm = GetLocalSearchAlgorithm();

         if (_innerAlgorithm == null)
         {
            throw new InvalidOperationException($"Bad configuration format: " +
               $"{nameof(Solver.WorkingConfiguration)}.");
         }

         _neighborhood.TabuList = new List<TabuMove>();
         _innerAlgorithm.AcceptImprovementsOnly = false;
         _innerAlgorithm.Provider = Provider;
      }

      internal override async Task PerformStep()
      {
         await _innerAlgorithm.Start();

         //if (Solver.BestSolution.Cost < Solver.PreviousStageSolutionCost)
         //{
         //   _noImprovementsCount++;
         //}

         bool isBetterThanPreviousBestSolution =
            Solver.Problem.CompareSolutionsCost(Solver.BestSolution.Cost, Solver.PreviousStageSolutionCost);

         if (!isBetterThanPreviousBestSolution)
         {
            _noImprovementsCount++;
         }
      
         AspirationCriteria();
         _currentIteration++;
      }

      internal override void OnError()
      {
         base.OnError();
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
         return _currentIteration == _maxIterations || 
            _noImprovementsCount == _maxIterations || 
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}