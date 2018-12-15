//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 15/12/2018
//

using CityScover.Commons;
using CityScover.Engine.Algorithms.LocalSearches;
using CityScover.Engine.Algorithms.Neighborhoods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
      private bool _canDoImprovements;
      private bool _shouldRunImprovement;
      private ICollection<ToSolution> _solutionsHistory;
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

      #region Internal properties
      public int ImprovementsCount { get; private set; }
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

      private void IncrementTabuMovesExpirations()
      {
         _tabuList.ToList().ForEach(reverseMove =>
         {
            reverseMove.Expiration++;

            var firstRoute = GetPointsKey(reverseMove.FirstEdgeId);
            var secondRoute = GetPointsKey(reverseMove.SecondEdgeId);

            string message = MessagesRepository.GetMessage(MessageCode.TabuSearchMovesLocked,
               $"({firstRoute.PointFrom}, {firstRoute.PointTo})",
               $"({secondRoute.PointFrom}, {secondRoute.PointTo})",
               $"{reverseMove.Expiration}",
               $"{_tenure}");

            Console.ForegroundColor = ConsoleColor.Magenta;
            SendMessage(message);
            Console.ForegroundColor = ConsoleColor.Gray;
         });
      }

      private void LockMove(Tuple<int, int> reverseMove)
      {
         var (firstEdgeId, secondEdgeId) = reverseMove;
         _tabuList.Add(new TabuMove(firstEdgeId, secondEdgeId));

         var firstRoute = GetPointsKey(firstEdgeId);
         var secondRoute = GetPointsKey(secondEdgeId);
         string message = MessagesRepository.GetMessage(MessageCode.TabuSearchMoveLocked,
            $"({firstRoute.PointFrom}, {firstRoute.PointTo})",
            $"({secondRoute.PointFrom}, {secondRoute.PointTo})");

         Console.ForegroundColor = ConsoleColor.Magenta;
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
               string message = MessagesRepository.GetMessage(MessageCode.TabuSearchMoveUnlocked,
                  $"({firstRoute.PointFrom}, {firstRoute.PointTo})",
                  $"({secondRoute.PointFrom}, {secondRoute.PointTo})",
                  $"{reverseMove.Expiration}");

               Console.ForegroundColor = ConsoleColor.Magenta;
               SendMessage(message);
               Console.ForegroundColor = ConsoleColor.Gray;
            }
         }
      }

      private IEnumerable<Algorithm> GetLocalSearchAlgorithms()
      {
         var childrenFlow = Solver.CurrentStage.Flow.ChildrenFlows;

         foreach (var flow in childrenFlow)
         {
            foreach (var algorithm in Solver.GetImprovementAlgorithms(flow.ChildrenFlows))
            {
               yield return algorithm;
            }
         }
      }

      private async Task RunImprovement()
      {
         foreach (var algorithm in GetLocalSearchAlgorithms())
         {
            algorithm.Provider = Provider;

            var solutionToImprove = (_tabuBestSolution.Cost < Solver.BestSolution.Cost)
               ? Solver.BestSolution
               : _tabuBestSolution;

            await _innerAlgorithm.RunImprovement(algorithm, solutionToImprove, Type);
            ImprovementsCount++;
         }
      }

      private async Task StartLocalSearch()
      {
         Solver.CurrentAlgorithm = _innerAlgorithm.Type;
         Task algorithmTask = Task.Run(_innerAlgorithm.Start);

         try
         {
            await algorithmTask.ConfigureAwait(false);
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

      private bool AspirationCriteria(in ToSolution neighborhoodSolution)
      {
         // Aspiration criteria
         bool isBetterThanCurrentBestSolution = Solver.Problem
            .CompareSolutionsCost(neighborhoodSolution.Cost, _tabuBestSolution.Cost);

         if (!isBetterThanCurrentBestSolution)
         {
            _noImprovementsCount++;
            _shouldRunImprovement = _noImprovementsCount == _maxDeadlockIterations;

            return false;
         }

         _tabuBestSolution = neighborhoodSolution;
         _innerAlgorithm.CurrentBestSolution = neighborhoodSolution;

         return true;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _maxIterations = Parameters[ParameterCodes.MaxIterations];
         _canDoImprovements = Parameters[ParameterCodes.CanDoImprovements];
         _maxDeadlockIterations = Parameters[ParameterCodes.TABUmaxDeadlockIterations];
         int tabuTenureFactor = Parameters[ParameterCodes.TABUtenureFactor];

         if (tabuTenureFactor <= Utils.MinTabuTenureFactor)
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
         _solutionsHistory = new Collection<ToSolution>();
         ImprovementsCount = default;
         _innerAlgorithm.AcceptImprovementsOnly = false;
         _innerAlgorithm.Provider = Provider;
         Solver.PreviousStageSolutionCost = Solver.BestSolution.Cost;
         _currentSolution = Solver.BestSolution;
         _tabuBestSolution = Solver.BestSolution;

         if (Solver.IsMonitoringEnabled)
         {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            SendMessage(MessageCode.TabuSearchStart, _currentSolution.Id, _currentSolution.Cost);
            Console.ForegroundColor = ConsoleColor.Gray;
         }
      }

      protected override async Task PerformStep()
      {
         // Runs the local search algorithm.
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
         _solutionsHistory.Add(neighborhoodBestSolution);
         _innerAlgorithm.ResetState();

         if (AspirationCriteria(neighborhoodBestSolution))
         {
            _currentIteration++;
            return;
         }

         // If move is prohibited, do nothing.
         Tuple<int, int> neighborhoodBestSolutionMove = neighborhoodBestSolution.Move;
         TabuMove forbiddenMove = _tabuList
            .FirstOrDefault(move => move.FirstEdgeId == neighborhoodBestSolutionMove.Item1 &&
                                    move.SecondEdgeId == neighborhoodBestSolutionMove.Item2);

         if (forbiddenMove == null)
         {
            _innerAlgorithm.CurrentBestSolution = neighborhoodBestSolution;
            IncrementTabuMovesExpirations();
            LockMove(neighborhoodBestSolution.Move);
         }
         else
         {
            IncrementTabuMovesExpirations();
         }

         // Remove from Tabu List expired moves.
         UnlockExpiredMoves();

         if (_canDoImprovements && _shouldRunImprovement)
         {
            await RunImprovement();
            _shouldRunImprovement = default;
            _noImprovementsCount = default;
            _innerAlgorithm.CurrentBestSolution = Solver.BestSolution;
         }
         _currentIteration++;
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _tabuBestSolution;
         Console.ForegroundColor = ConsoleColor.DarkMagenta;
         SendMessage(MessageCode.TabuSearchImprovementsPerformed, ImprovementsCount);
         SendMessage(MessageCode.TabuSearchStop, _tabuBestSolution.Id, _tabuBestSolution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;
         SendMessage(ToSolution.SolutionCollectionToString(_solutionsHistory));
      }

      internal override bool StopConditions()
      {
         return _currentIteration == _maxIterations || base.StopConditions();
      }
      #endregion
   }
}
