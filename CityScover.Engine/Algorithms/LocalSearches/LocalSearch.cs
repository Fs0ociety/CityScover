//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 14/10/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms
{
   internal class LocalSearch : Algorithm
   {
      #region Private fields
      private int _previousSolutionCost;
      private int _currentSolutionCost;
      private TOSolution _bestSolution;
      private NeighborhoodFacade _neighborhoodFacade;
      #endregion

      #region Constructors
      internal LocalSearch(Neighborhood neighborhood)
         : this(neighborhood, null)
      {
      }

      public LocalSearch(Neighborhood neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
         _neighborhoodFacade = new NeighborhoodFacade(neighborhood);
      }
      #endregion

      #region Private methods
      private TOSolution GetBest(IEnumerable<TOSolution> neighborhood, TOSolution currentSolution, byte? maxImprovementsCount)
      {
         if (neighborhood == null || currentSolution == null)
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
         Solver.PreviousStageSolutionCost = Solver.BestSolution.Cost;
         _bestSolution = Solver.BestSolution;
         _currentSolutionCost = _bestSolution.Cost;
         _previousSolutionCost = default;
      }

      internal override async Task PerformStep()
      {
         var currentNeighborhood = _neighborhoodFacade.GenerateNeighborhood(_bestSolution);

         foreach (var neighborSolution in currentNeighborhood)
         {
            Solver.EnqueueSolution(neighborSolution);
            await Task.Delay(250).ConfigureAwait(continueOnCapturedContext: false);

            if (Solver.IsMonitoringEnabled)
            {
               // Notifica gli observers.
               Provider.NotifyObservers(neighborSolution);
            }
         }
         await Task.WhenAll(Solver.AlgorithmTasks.Values);

         // Ora sono sicuro di avere tutte le soluzioni dell'intorno valorizzate.

         // TODO: come gestire eventuali best differenti ? (es. Best Improvement se maxImprovementsCount è null,
         // altrimenti First Improvement se maxImprovementsCount = 1, K Improvment se maxImprovementsCount = k.
         // Per come è fatta adesso, sarà sempre Best Improvement.

         // Se siamo ispirati (come no) lo faremo.

         // TODO
         // Per l'invocazione dell'algoritmo di miglioramento Lin Kernighan utilizzare due ulteriori variabili:
         // 1. Un delta fissato all'inzio che indica una variazione di costo generica.
         // 2. Un contatore che mi tiene traccia di quante volte ho migliorato con una variazione (differenza tra i costi delle due soluzioni) inferiore al delta.
         //
         // Pertanto, l'invocazione dell'algoritmo avviene quando il contatore raggiunge una soglia fissata. (e.g. dopo 4 giri della PerformStep())

         var solution = GetBest(currentNeighborhood, _bestSolution, null);
         _previousSolutionCost = _currentSolutionCost;

         if (AcceptImprovementsOnly)
         {
            bool isBetterThanCurrentBestSolution = Solver.Problem.CompareSolutionsCost(solution.Cost, _bestSolution.Cost);
            if (isBetterThanCurrentBestSolution)
            {
               _bestSolution = solution;
               _currentSolutionCost = solution.Cost;
            }
         }
         else
         {
            _bestSolution = solution;
            _currentSolutionCost = solution.Cost;
         }
      }

      internal override void OnError(Exception exception)
      {
         base.OnError(exception);

         // Da gestire timeSpent (probabilmente con metodo che somma i tempi di tutti i nodi).
         Result resultError =
            new Result(_bestSolution, CurrentAlgorithm, null, Result.Validity.Invalid);
         resultError.ResultFamily = AlgorithmFamily.LocalSearch;
         Solver.Results.Add(resultError);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _bestSolution;
      }

      internal override void OnTerminated()
      {
         // Da gestire timeSpent (probabilmente con metodo che somma i tempi di tutti i nodi).
         Result validResult =
            new Result(_bestSolution, CurrentAlgorithm, null, Result.Validity.Valid);
         validResult.ResultFamily = AlgorithmFamily.LocalSearch;
         Solver.Results.Add(validResult);
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost == _currentSolutionCost ||
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}