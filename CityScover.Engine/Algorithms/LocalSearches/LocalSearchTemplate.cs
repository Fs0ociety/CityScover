
//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 14/11/2018
//

using CityScover.Commons;
using CityScover.Engine.Algorithms.CustomAlgorithms;
using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Algorithms.VariableDepthSearch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms
{
   internal class LocalSearchTemplate : Algorithm
   {
      #region Private fields
      private int _previousSolutionCost;
      private int _currentSolutionCost;
      private int _iterationsWithoutImprovement;
      private bool _shouldRunImprovementAlgorithm;
      private int _improvementThreshold;
      private int _maxIterationsWithoutImprovements;
      private TOSolution _bestSolution;
      private NeighborhoodFacade _neighborhoodFacade;
      private ICollection<TOSolution> _solutionsHistory;
      #endregion

      #region Constructors
      internal LocalSearchTemplate(Neighborhood neighborhood)
         : this(neighborhood, provider: null)
      {
      }

      public LocalSearchTemplate(Neighborhood neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
         Type = neighborhood.Type;
         _neighborhoodFacade = new NeighborhoodFacade(neighborhood, this);
      }
      #endregion

      #region Internal properties
      internal bool CanDoImprovements { get; set; }
      #endregion

      #region Private methods
      private TOSolution GetBest(IEnumerable<TOSolution> neighborhood, TOSolution currentSolution, byte? maxImprovementsCount)
      {
         if (neighborhood is null || currentSolution is null)
         {
            throw new ArgumentNullException(nameof(neighborhood));
         }
         if (maxImprovementsCount.HasValue && maxImprovementsCount == 0)
         {
            throw new ArgumentException("maxImprovementsCount can not have value 0.");
         }

         TOSolution bestSolution = currentSolution;
         byte currentImprovement = default;

         foreach (var solution in neighborhood)
         {
            if (maxImprovementsCount.HasValue && currentImprovement > maxImprovementsCount)
            {
               break;
            }

            bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, bestSolution.Cost);
            if (isBetterThanCurrentBestSolution)
            {
               bestSolution = solution;
               currentImprovement++;
            }
         }

         return bestSolution;
      }

      private IEnumerable<Algorithm> GetImprovementAlgorithms()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms is null)
         {
            yield return null;
         }

         Algorithm algorithm = default;
         foreach (var child in childrenAlgorithms)
         {
            algorithm = Solver.GetAlgorithm(child.CurrentAlgorithm);

            if (algorithm is LinKernighan lk)
            {
               algorithm = lk;
               lk.MaxSteps = child.RunningCount;
               lk.CurrentBestSolution = _bestSolution;
            }
            else if (algorithm is HybridNearestDistance hnd)
            {
               algorithm = hnd;
            }

            if (algorithm is null)
            {
               throw new NullReferenceException(nameof(algorithm));
            }
            algorithm.Parameters = child.AlgorithmParameters;
            algorithm.Provider = Provider;
         
            yield return algorithm;
         }
      }

      private async Task RunImprovementAlgorithms()
      {
         foreach (var algorithm in GetImprovementAlgorithms())
         {
            if (algorithm is null)
            {
               throw new InvalidOperationException($"Bad configuration format: " +
                  $"{nameof(Solver.WorkingConfiguration)}.");
            }

            Solver.CurrentAlgorithm = algorithm.Type;
            await Task.Run(() => algorithm.Start());
            Solver.CurrentAlgorithm = Type;
            _bestSolution = Solver.BestSolution;
            _shouldRunImprovementAlgorithm = false;
            _iterationsWithoutImprovement = 0;

            // Verificare se necessario eseguire i successivi algoritmi dello StageFlow presenti tra i ChildrenFlows.
            // Usare un if-break per questo scopo sotto questo commento, dopo la terminazione dell'algoritmo.
            // Senza questo if verrebbero eseguiti a prescindere in sequenza tutti gli algoritmi presenti tra i ChildrenFlows, 
            // anche se non fosse necessario avviare i successivi.
         }
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _solutionsHistory = new Collection<TOSolution>();
         Solver.PreviousStageSolutionCost = Solver.BestSolution.Cost;
         CanDoImprovements = Parameters[ParameterCodes.CanDoImprovements];
         _improvementThreshold = Parameters[ParameterCodes.LKimprovementThreshold];
         _maxIterationsWithoutImprovements = Parameters[ParameterCodes.LSmaxRunsWithNoImprovements];

         _bestSolution = Solver.BestSolution;
         _solutionsHistory.Add(_bestSolution);
         SendMessage(MessageCode.LSStartSolution, _bestSolution.Id, _bestSolution.Cost);
         _currentSolutionCost = _bestSolution.Cost;
         _previousSolutionCost = default;
         _iterationsWithoutImprovement = default;
         _shouldRunImprovementAlgorithm = default;
      }

      internal override async Task PerformStep()
      {
         if (CanDoImprovements && _shouldRunImprovementAlgorithm)
         {
            await RunImprovementAlgorithms();
            SendMessage(MessageCode.LSResumeSolution, _bestSolution.Id, _bestSolution.Cost);
         }

         var currentNeighborhood = _neighborhoodFacade.GenerateNeighborhood(_bestSolution);

         foreach (var neighborSolution in currentNeighborhood)
         {
            SendMessage(MessageCode.LSNewNeighborhoodMove, neighborSolution.Id, CurrentStep + 1);
            SendMessage(neighborSolution.Description);
            Solver.EnqueueSolution(neighborSolution);
            await Task.Delay(Utils.DelayTask).ConfigureAwait(continueOnCapturedContext: false);

            if (Solver.IsMonitoringEnabled)
            {
               // Notifica gli observers.
               Provider.NotifyObservers(neighborSolution);
            }
         }
         await Task.WhenAll(Solver.AlgorithmTasks.Values);
         var solution = GetBest(currentNeighborhood, _bestSolution, null);
         Console.ForegroundColor = ConsoleColor.Green;
         SendMessage(MessageCode.LSNeighborhoodBest, solution.Id, solution.Cost);
         Console.ForegroundColor = ConsoleColor.Gray;

         _previousSolutionCost = _currentSolutionCost;

         if (AcceptImprovementsOnly)
         {
            bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, _bestSolution.Cost);
            if (isBetterThanCurrentBestSolution)
            {
               SendMessage(MessageCode.LSBestFound, solution.Cost, _bestSolution.Cost);
               _bestSolution = solution;
               _solutionsHistory.Add(solution);
               _currentSolutionCost = solution.Cost;

               if (CanDoImprovements)
               {
                  var delta = _currentSolutionCost - _previousSolutionCost;
                  if (delta < _improvementThreshold)
                  {
                     _iterationsWithoutImprovement++;
                     _shouldRunImprovementAlgorithm = _iterationsWithoutImprovement >= _maxIterationsWithoutImprovements;
                  }
               }
            }
         }
         else
         {
            _bestSolution = solution;
            _currentSolutionCost = solution.Cost;
         }
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _bestSolution;
         Console.ForegroundColor = ConsoleColor.Yellow;
         SendMessage(MessageCode.LSFinish, _currentSolutionCost);
         Console.ForegroundColor = ConsoleColor.Gray;
         SendMessage(TOSolution.SolutionCollectionToString(_solutionsHistory));
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost == _currentSolutionCost ||
            Status == AlgorithmStatus.Error;
      }
      #endregion
   }
}