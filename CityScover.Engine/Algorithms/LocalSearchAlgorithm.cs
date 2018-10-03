//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 04/10/2018
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms
{
   internal class LocalSearchAlgorithm : Algorithm
   {
      private int _previousSolutionCost;
      private int _currentSolutionCost;
      private TOSolution _bestSolution;
      private Neighborhood _neighborhood;

      #region Constructors
      internal LocalSearchAlgorithm(Neighborhood neighborhood)
         : this(neighborhood, null)
      {
      }

      public LocalSearchAlgorithm(Neighborhood neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
         _neighborhood = neighborhood;
      }
      #endregion

      #region Private methods
      private TOSolution GetBest(IEnumerable<TOSolution> neighborhood, TOSolution currentSolution, byte? maxImprovementsCount)
      {
         if (neighborhood == null || currentSolution == null)
         {
            throw new ArgumentNullException(nameof(Neighborhood));
         }

         if (maxImprovementsCount.HasValue && maxImprovementsCount == 0)
         {
            throw new ArgumentException("Se maxImprovementsCount è valorizzato, non può avere valore 0.");
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
               _currentSolutionCost = solution.Cost;
               currentImprovement++;
            }
         }

         return bestSolution;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         _bestSolution = Solver.BestSolution;
         _currentSolutionCost = _bestSolution.Cost;         
         _previousSolutionCost = default;
      }

      internal override async Task PerformStep()
      {
         var currentNeighborhood = _neighborhood.GetAllMoves(_bestSolution);

         // TODO: Capire come gestire il monitoraggio dell'ExecutionReporter.
         Debug.WriteLine("Starting Local Search Validation");
         foreach (var neighborhoodSolution in currentNeighborhood)
         {
            // Notifica gli observers.
            notifyingFunc.Invoke(neighborhoodSolution);
            await Task.Delay(100).ConfigureAwait(continueOnCapturedContext: false);
         }

         await Task.WhenAll(Solver.AlgorithmTasks.ToArray());

         // Ora sono sicuro di avere tutte le soluzioni dell'intorno valorizzate.

         // TODO: come gestire eventuali best differenti ? (es. Best Improvement se maxImprovementsCount è null,
         // altrimenti First Improvement se maxImprovementsCount = 1, K Improvment se maxImprovementsCount = k.
         // Per come è fatta adesso, sarà sempre Best Improvement.

         // Se siamo ispirati (come no) lo faremo.
         var solution = GetBest(currentNeighborhood, _bestSolution, null);         
         _previousSolutionCost = _currentSolutionCost;

         bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, _bestSolution.Cost);
         if (isBetterThanCurrentBestSolution)
         {
            _bestSolution = solution;
            _currentSolutionCost = solution.Cost;
         }
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost == _currentSolutionCost || 
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}
