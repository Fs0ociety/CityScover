//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 26/09/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine.Algorithms
{
   internal class LocalSearchAlgorithm : Algorithm
   {
      private double _previousSolutionCost;
      private double _currentSolutionCost;
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

            if (solution.Cost < bestSolution.Cost)
            {
               bestSolution = solution;
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
      }

      internal override void PerformStep()
      {
         ICollection<TOSolution> processedNeighborhood = new Collection<TOSolution>();

         var currentNeighborhood = _neighborhood.GetAllMoves(_bestSolution);
         foreach (var neighborhoodSolution in currentNeighborhood)
         {
            // Notifica gli observers.
            notifyingFunc.Invoke(neighborhoodSolution);
         }

         // Resta in attesa che arrivino elementi dalla coda base del Solver.
         var processedSolutions = Solver.GetProcessedSolutions();
         foreach (var processedSolution in processedSolutions)
         {
            processedNeighborhood.Add(processedSolution);
         }

         // Ora sono sicuro di avere tutte le soluzioni dell'intorno valorizzate.

         //TODO: come gestire eventuali best differenti ? (es. Best Improvement se maxImprovementsCount è null,
         // altrimenti First Improvement se maxImprovementsCount = 1, K Improvment se maxImprovementsCount = k.
         // Per come è fatta adesso, sarà sempre Best Improvement.
         var solution = GetBest(processedNeighborhood, _bestSolution, null);
                  
         _previousSolutionCost = _bestSolution.Cost;
         if (solution.Cost < _bestSolution.Cost)
         {
            _bestSolution = solution;
         }
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost != _currentSolutionCost ||
            _status == AlgorithmStatus.Terminating;
      }
      #endregion
   }
}
