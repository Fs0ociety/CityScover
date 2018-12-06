//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 06/12/2018
//

using CityScover.Engine.Algorithms.LocalSearches;
using CityScover.Engine.Algorithms.Neighborhoods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal class TabuSearch2 : Algorithm
   {
      #region Private fields
      private readonly TabuSearchNeighborhood2 _neighborhood;
      private LocalSearchTemplate _innerAlgorithm;
      private IList<TabuMove> _tabuList;
      private ToSolution _neighborhoodBestSolution;
      private ToSolution _tabuBestSolution;
      private int _tenure;
      private int _maxIterations;
      private int _maxDeadlockIterations;
      private int _noImprovementsCount;
      private int _currentIteration;
      //private ICollection<ToSolution> _solutionsHistory;
      #endregion

      #region Constructors
      internal TabuSearch2(Neighborhood neighborhood) : this(neighborhood, null)
         => Type = AlgorithmType.TabuSearch;

      internal TabuSearch2(Neighborhood neighborhood, AlgorithmTracker provider) : base(provider)
      {
         if (neighborhood is TabuSearchNeighborhood2 tbNeighborhood)
         {
            _neighborhood = tbNeighborhood;
         }
      }
      #endregion

      #region Internal properties
      internal IList<TabuMove> TabuList => _tabuList;
      #endregion

      #region Private methods
      private int CalculateTabuTenure(int problemSize, int factor)
         => (problemSize + factor - 1) / factor;

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
         _neighborhood.Algorithm = this;
         Algorithm algorithm = Solver.GetAlgorithm(flow.CurrentAlgorithm, _neighborhood);

         if (algorithm is LocalSearchTemplate ls)
         {
            algorithm.Parameters = flow.AlgorithmParameters;
            ls.CurrentBestSolution = _neighborhoodBestSolution;
            _innerAlgorithm = ls;
         }

         return _innerAlgorithm;
      }

      private (int PointFrom, int PointTo) GetPointsKey(int edgeId)
      {
         var edge = Solver.CityMapGraph.Edges
            .FirstOrDefault(e => e.Entity.Id == edgeId);

         return (edge.Entity.PointFrom.Id, edge.Entity.PointTo.Id);
      }

      private void IncrementTabuMovesExpirations()
      {
         _tabuList.ToList().ForEach(move =>
         {
            move.Expiration++;
            var firstRoute = GetPointsKey(move.FirstEdgeId);
            var secondRoute = GetPointsKey(move.SecondEdgeId);

            string message = MessagesRepository.GetMessage(MessageCode.TSMovesLocked,
               $"({firstRoute.PointFrom}, {firstRoute.PointTo})",
               $"({secondRoute.PointFrom}, {secondRoute.PointTo})",
               $"{move.Expiration}",
               $"{_tenure}");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            SendMessage(message);
            Console.ForegroundColor = ConsoleColor.Gray;
         });
      }

      private void LockMove(Tuple<int, int> move)
      {
         var (firstEdgeId, secondEdgeId) = move;
         IncrementTabuMovesExpirations();
         _tabuList.Add(new TabuMove(firstEdgeId, secondEdgeId));

         var firstRoute = GetPointsKey(firstEdgeId);
         var secondRoute = GetPointsKey(secondEdgeId);
         string message = MessagesRepository.GetMessage(MessageCode.TSMoveLocked,
            $"({firstRoute.PointFrom}, {firstRoute.PointTo})",
            $"({secondRoute.PointFrom}, {secondRoute.PointTo})");

         Console.ForegroundColor = ConsoleColor.DarkYellow;
         SendMessage(message);
         Console.ForegroundColor = ConsoleColor.Gray;
      }

      private void UnlockExpiredMoves()
      {
         #region LINQ version
         // Using LINQ with generic Predicate<T> delegate to remove items in Tabu list.
         //_tabuList.ToList().RemoveAll(tabuMove => tabuMove.Expiration >= _tenure);
         #endregion

         // Using classic foreach, iterating over a copy of the Tabu list.
         foreach (var reverseMove in _tabuList)
         {
            if (reverseMove.Expiration >= _tenure)
            {
               _tabuList.Remove(reverseMove);

               var firstRoute = GetPointsKey(reverseMove.FirstEdgeId);
               var secondRoute = GetPointsKey(reverseMove.SecondEdgeId);
               string message = MessagesRepository.GetMessage(MessageCode.TSMoveUnlocked,
                  $"({firstRoute.PointFrom}, {firstRoute.PointTo})",
                  $"({secondRoute.PointFrom}, {secondRoute.PointTo})",
                  $"{reverseMove.Expiration}");

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

      private void RunLocalSearch2()
      {
         Solver.CurrentAlgorithm = _innerAlgorithm.Type;
         Task localSearchTask = Task.Run(_innerAlgorithm.Start);
         try
         {
            localSearchTask.Wait();
         }
         catch (AggregateException ae)
         {
            OnError(ae.InnerException);
         }
         finally
         {
            Solver.CurrentAlgorithm = Type;
         }
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         //_solutionsHistory = new Collection<ToSolution>();
         _maxIterations = Solver.CurrentStage.Flow.RunningCount;
         _maxDeadlockIterations = Parameters[ParameterCodes.TABUmaxDeadlockIterations];
         int tabuTenureFactor = Parameters[ParameterCodes.TABUtenureFactor];

         if (tabuTenureFactor <= 1)
         {
            throw new InvalidOperationException("Bad configuration format: " +
               $"{nameof(ParameterCodes.TABUtenureFactor)}.");
         }

         _tenure = CalculateTabuTenure(Solver.ProblemSize, tabuTenureFactor);
         _innerAlgorithm = GetLocalSearchAlgorithm();

         if (_innerAlgorithm is null)
         {
            throw new InvalidOperationException("Bad configuration format: " +
               $"{nameof(Solver.WorkingConfiguration)}.");
         }

         _tabuList = new List<TabuMove>();
         _innerAlgorithm.AcceptImprovementsOnly = false;
         _innerAlgorithm.Provider = Provider;
         //Solver.PreviousStageSolutionCost = Solver.BestSolution.Cost;
         _neighborhoodBestSolution = Solver.BestSolution;
         _tabuBestSolution = Solver.BestSolution;
         Console.ForegroundColor = ConsoleColor.Magenta;
         SendMessage(MessageCode.TSStarting, _neighborhoodBestSolution.Id, _neighborhoodBestSolution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;
      }

      protected override async Task PerformStep()
      {
         //RunLocalSearch2();

         await RunLocalSearch().ConfigureAwait(continueOnCapturedContext: false);
         _neighborhoodBestSolution = _innerAlgorithm.CurrentBestSolution;

         // Aspiration criteria
         bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(
            _innerAlgorithm.CurrentBestSolution.Cost,
            _tabuBestSolution.Cost);

         if (isBetterThanCurrentBestSolution)
         {
            _tabuBestSolution = _innerAlgorithm.CurrentBestSolution;
         }
         else
         {
            _noImprovementsCount++;
         }

         // Inserisco la mossa Best fornita dalla Local Search nella Tabu List.
         LockMove(_innerAlgorithm.Move);

         _currentIteration++;
         _innerAlgorithm.ResetState();
         _innerAlgorithm.CurrentBestSolution = _neighborhoodBestSolution;

         // Remove from Tabu List expired moves.
         UnlockExpiredMoves();
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _tabuBestSolution;
         //SendMessage(ToSolution.SolutionCollectionToString(_solutionsHistory));
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
