//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/12/2018
//

using CityScover.Engine.Algorithms.LocalSearches;
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
      private Neighborhood<ToSolution> _neighborhood;
      private LocalSearchTemplate _innerAlgorithm;
      private IList<TabuMove> _tabuList;
      private ToSolution _currentSolution;
      private ToSolution _tabuBestSolution;
      private int _tenure;
      private int _maxIterations;
      private int _maxDeadlockIterations;
      private int _noImprovementsCount;
      private int _currentIteration;
      //private ICollection<ToSolution> _solutionsHistory;
      #endregion

      #region Constructors
      internal TabuSearch(Neighborhood<ToSolution> neighborhood) : this(neighborhood, null)
         => Type = AlgorithmType.TabuSearch;

      internal TabuSearch(Neighborhood<ToSolution> neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
         if (neighborhood is TwoOptNeighborhood tbNeighborhood)
         {
            _neighborhood = tbNeighborhood;
         }
      }
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

         _neighborhood = NeighborhoodFactory.CreateNeighborhood(flow.CurrentAlgorithm);
         Algorithm algorithm = Solver.GetAlgorithm(flow.CurrentAlgorithm, _neighborhood);

         if (algorithm is LocalSearchTemplate ls)
         {
            algorithm.Parameters = flow.AlgorithmParameters;
            ls.CurrentBestSolution = _currentSolution;
            _innerAlgorithm = ls;
         }
         else
         {
            throw new NullReferenceException(nameof(algorithm));
         }

         return _innerAlgorithm;
      }

      private (int PointFrom, int PointTo) GetPointsKey(int edgeId)
      {
         var edge = Solver.CityMapGraph.Edges
            .FirstOrDefault(e => e.Entity.Id == edgeId);

         return (edge.Entity.PointFrom.Id, edge.Entity.PointTo.Id);
      }

      private void IncrementTabuMovesExpirations(TabuMove insertedMove)
      {
         _tabuList.ToList().ForEach(move =>
         {
            if (insertedMove is null)
            {
               return;
            }

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
         foreach (var reverseMove in _tabuList.ToList())
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

      private async Task StartLocalSearch()
      {
         Solver.CurrentAlgorithm = _innerAlgorithm.Type;
         Task localSearchTask = Task.Run(_innerAlgorithm.Start);

         try
         {
            await localSearchTask.ConfigureAwait(false);
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
         Solver.PreviousStageSolutionCost = Solver.BestSolution.Cost;
         _currentSolution = Solver.BestSolution;
         _tabuBestSolution = Solver.BestSolution;
         Console.ForegroundColor = ConsoleColor.Magenta;
         SendMessage(MessageCode.TSStarting, _currentSolution.Id, _currentSolution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;
      }

      protected override async Task PerformStep()
      {
         await StartLocalSearch();

         ToSolution innerAlgorithmBestSolution = _innerAlgorithm.CurrentBestSolution;
         ToSolution neighborhoodBestSolution = new ToSolution()
         {
            Id = innerAlgorithmBestSolution.Id,
            SolutionGraph = innerAlgorithmBestSolution.SolutionGraph.DeepCopy(),
            Move = Tuple.Create(innerAlgorithmBestSolution.Move.Item1, innerAlgorithmBestSolution.Move.Item2),
            Cost = innerAlgorithmBestSolution.Cost,
            Penalty = innerAlgorithmBestSolution.Penalty,            
            Description = innerAlgorithmBestSolution.Description,
         }; 
         _innerAlgorithm.ResetState();

         // Aspiration criteria
         bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(
            neighborhoodBestSolution.Cost,
            _tabuBestSolution.Cost);

         if (isBetterThanCurrentBestSolution)
         {
            _tabuBestSolution = neighborhoodBestSolution;
            _innerAlgorithm.CurrentBestSolution = neighborhoodBestSolution;
            _currentIteration++;

            // If aspiration criteria is successful, go straight on 
            // checking the stopping conditions and nothing more.
            return;
         }
         else
         {
            _noImprovementsCount++;
         }

         // If move is prohibited, do nothing.
         Tuple<int, int> neighborhoodBestSolutionMove = neighborhoodBestSolution.Move;
         TabuMove forbiddenMove = _tabuList
            .FirstOrDefault(move => move.FirstEdgeId == neighborhoodBestSolutionMove.Item1 &&
                                    move.SecondEdgeId == neighborhoodBestSolutionMove.Item2);

         if (forbiddenMove == null)
         {
            _innerAlgorithm.CurrentBestSolution = neighborhoodBestSolution;
            _tabuList.ToList().ForEach(move => move.Expiration++);

            // Insert the current move into tabu list.
            LockMove(neighborhoodBestSolution.Move);
         }
         else
         {
            _tabuList.ToList().ForEach(move => move.Expiration++);

            // Increment the expiration count of moves. If forbiddenMove is null, 
            // move has just been inserted, and so don't increment expiration count
            // for this move.
            //IncrementTabuMovesExpirations(forbiddenMove);
         }

         // Remove from Tabu List expired moves.
         UnlockExpiredMoves();
         _currentIteration++;
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
