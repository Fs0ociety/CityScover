//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 05/12/2018
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
         _tabuList = new List<TabuMove>();
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
         await RunLocalSearch().ConfigureAwait(continueOnCapturedContext: false);
         _neighborhoodBestSolution = _innerAlgorithm.CurrentBestSolution;

         // Aspiration criteria
         bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(_innerAlgorithm.CurrentBestSolution.Cost, _tabuBestSolution.Cost);
         if (isBetterThanCurrentBestSolution)
         {
            _tabuBestSolution = _innerAlgorithm.CurrentBestSolution;
         }
         else
         {
            _noImprovementsCount++;
         }

         // Metto la mossa Best fornita dalla Local Search nella Tabu List.
         //var (firstEdgeId, secondEdgeId) = _innerAlgorithm.Move;
         LockMove(_innerAlgorithm.Move);
         //_tabuList.Add(new TabuMove(firstEdgeId, secondEdgeId));
         //IncrementTabuMovesExpirations();

         _currentIteration++;
         _innerAlgorithm.ResetState();
         _innerAlgorithm.CurrentBestSolution = _neighborhoodBestSolution;
      }

      private void LockMove(Tuple<int, int> move)
      {
         throw new NotImplementedException();
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
