//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 23/09/2018
//

namespace CityScover.Engine.Algorithms
{
   internal class TOLocalSearchAlgorithm : Algorithm
   {
      private double _previousSolutionCost;
      private double _currentSolutionCost;
      private TOSolution _bestSolution;

      private TONeighborhood _neighborhoodWorker;

      #region Constructors
      internal TOLocalSearchAlgorithm(TONeighborhood neighborhood)
         : this(neighborhood, null)
      {
      }

      public TOLocalSearchAlgorithm(TONeighborhood neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
         _neighborhoodWorker = neighborhood;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

         //TODO: prendere sempre la soluzione dal Solver (la best solution) , che è quella
         // generata allo step precedente, oppure passarla come parametro ? 
         _bestSolution = Solver.BestSolution;
      }

      internal override void PerformStep()
      {
         var currentNeighborhood = _neighborhoodWorker.GetAllMoves(_bestSolution);

         //TODO: come gestire eventuali best differenti ? (es. Best Improvement se maxImprovementsCount è null,
         // altrimenti First Improvement se maxImprovementsCount = 1, K Improvment se maxImprovementsCount = k.
         // Per come è fatta adesso, sarà sempre Best Improvement.
         var solution = _neighborhoodWorker.GetBest(currentNeighborhood, _bestSolution, null);

         // TODO più importante: qua solution.Cost è ancora a 0! Come gestire questa cosa con il nostro giro
         // SolverValidator - SolverEvaluator ?
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
