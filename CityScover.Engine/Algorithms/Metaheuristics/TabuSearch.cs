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

using CityScover.Engine.Algorithms.Neighborhoods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CityScover.Engine.Algorithms.LocalSearches;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal class TabuSearch : Algorithm
   {
      #region Private fields
      private readonly TabuSearchNeighborhood _neighborhood;
      private LocalSearchTemplate _innerAlgorithm;
      private TOSolution _currentBestSolution;
      private int _tenure;
      private int _maxIterations;
      private int _maxDeadlockIterations;
      private int _noImprovementsCount;
      private int _currentIteration;
      private ICollection<TOSolution> _solutionsHistory;
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

         var flow = childrenAlgorithms?.FirstOrDefault();
         if (flow is null)
         {
            return null;
         }

         _neighborhood.NeighborhoodWorker = NeighborhoodFactory.CreateNeighborhood(flow.CurrentAlgorithm);
         _neighborhood.Type = _neighborhood.NeighborhoodWorker.Type;
         Algorithm algorithm = Solver.GetAlgorithm(flow.CurrentAlgorithm, _neighborhood);

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

      private void IncrementTabuMovesExpirations()
      {
         _neighborhood.TabuList.ToList().ForEach(move =>
         {
            move.Expiration++;

            int pointFromId = Solver.CityMapGraph
               .Edges
               .Where(edge => edge.Entity.Id == move.FirstEdgeId)
               .Select(edge => edge.Entity.PointFrom.Id).FirstOrDefault();

            int pointToId = Solver.CityMapGraph
               .Edges
               .Where(edge => edge.Entity.Id == move.SecondEdgeId)
               .Select(edge => edge.Entity.PointTo.Id).FirstOrDefault();

            string message = MessagesRepository.GetMessage(MessageCode.TSMovesLocked,
               $"({pointFromId}, {pointToId})",
               $"{move.Expiration}",
               $"{_tenure}");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            SendMessage(message);
            Console.ForegroundColor = ConsoleColor.Gray;
         });
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

               int pointFromId = Solver.CityMapGraph
               .Edges
               .Where(edge => edge.Entity.Id == tabuMove.FirstEdgeId)
               .Select(edge => edge.Entity.PointFrom.Id).FirstOrDefault();

               int pointToId = Solver.CityMapGraph
                  .Edges
                  .Where(edge => edge.Entity.Id == tabuMove.SecondEdgeId)
                  .Select(edge => edge.Entity.PointTo.Id).FirstOrDefault();

               string message = MessagesRepository.GetMessage(MessageCode.TSMoveUnlocked,
                  $"({pointFromId}, {pointToId})",
                  $"{tabuMove.Expiration}");
               
               Console.ForegroundColor = ConsoleColor.Magenta;
               SendMessage(message);
               Console.ForegroundColor = ConsoleColor.Gray;
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
         _solutionsHistory = new Collection<TOSolution>();
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
         Solver.PreviousStageSolutionCost = Solver.BestSolution.Cost;
         _currentBestSolution = Solver.BestSolution;
         Console.ForegroundColor = ConsoleColor.Magenta;
         SendMessage(MessageCode.TSStarting, _currentBestSolution.Id, _currentBestSolution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;
      }

      internal override async Task PerformStep()
      {         
         await RunLocalSearch().ConfigureAwait(continueOnCapturedContext: false);

         bool isBetterThanPreviousBestSolution = Solver.Problem.CompareSolutionsCost(
            _innerAlgorithm.CurrentBestSolution.Cost,
            _currentBestSolution.Cost);

         if (isBetterThanPreviousBestSolution)
         {
            // La CurrentBestSolution fornita dalla LS è migliore della best corrente della Tabu, quindi
            // applico la mossa (faccio assegnamento)
            Console.ForegroundColor = ConsoleColor.Green;
            SendMessage(MessageCode.TSBestFound, _innerAlgorithm.CurrentBestSolution.Cost, _currentBestSolution.Cost);
            Console.ForegroundColor = ConsoleColor.Gray;
            _currentBestSolution = new TOSolution()
            {
               SolutionGraph = _innerAlgorithm.CurrentBestSolution.SolutionGraph.DeepCopy()
            };
            _solutionsHistory.Add(_currentBestSolution);
            var (firstEdgeId, secondEdgeId) = _innerAlgorithm.Move;
            _neighborhood.TabuList.Add(new TabuMove(firstEdgeId, secondEdgeId, expiration: 0));
            IncrementTabuMovesExpirations();
         }
         else
         {
            _noImprovementsCount++;
         }

         AspirationCriteria();
         _currentIteration++;
         _innerAlgorithm.ResetState();
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _currentBestSolution;
         SendMessage(TOSolution.SolutionCollectionToString(_solutionsHistory));
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