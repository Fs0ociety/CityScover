//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 06/10/2018
//

using CityScover.Engine.Algorithms.LocalSearches;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   // Optimizing tabu list size for the traveling salesman problem
   // https://www.sciencedirect.com/science/article/pii/S0305054897000300

   internal class TabuSearch : Algorithm
   {
      private LocalSearch _localSearchAlgorithm;
      private IList<TOSolution> _tabuList;
      private TOSolution _bestSolution;
      private int _currentIteration;
      private readonly int _maxImprovements;    // Si potrebbe gestire nello StageFlow della Configurazione
      private readonly int _maxIterations;      // Si potrebbe gestire nello StageFlow della Configurazione

      #region Constructors
      internal TabuSearch()
         : this(null)
      {
      }

      internal TabuSearch(AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      private IEnumerable<Algorithm> GetInnerAlgorithms()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms == null)
         {
            return null;
         }

         ICollection<Algorithm> algorithms = new Collection<Algorithm>();
         foreach (var children in childrenAlgorithms)
         {
            algorithms.Add(Solver.GetAlgorithm(children.CurrentAlgorithm));
         }

         return algorithms;
      }

      private LocalSearch GetLocalSearchAlgorithm()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms == null)
         {
            return null;
         }

         var algorithmFlow = childrenAlgorithms.FirstOrDefault();
         if (algorithmFlow == null)
         {
            return null;
         }

         switch (algorithmFlow.CurrentAlgorithm)
         {
            case AlgorithmType.TwoOpt:
               _localSearchAlgorithm = 
                  new LocalSearch(new TabuSearchTwoOptNeighborhood());
               break;

            case AlgorithmType.CitySwap:
               // TODO: Create CitySwap neighborhood object
               break;

            // Repeat for other Local Search algorithms...

            default:
               _localSearchAlgorithm = null;
               break;
         }

         return _localSearchAlgorithm;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

         _currentIteration = default;
         _bestSolution = Solver.BestSolution;
         _tabuList = new List<TOSolution>();
         _localSearchAlgorithm = GetLocalSearchAlgorithm();

         if (_localSearchAlgorithm == null)
         {
            throw new InvalidOperationException($"Bad configuration format: " +
               $"{nameof(Solver.WorkingConfiguration)}.");
         }

         _localSearchAlgorithm.AcceptImprovementsOnly = false;
      }

      internal override async Task PerformStep()
      {
         await Task.Run(() => _localSearchAlgorithm.PerformStep());

         if (!_tabuList.Any())
         {
            throw new InvalidOperationException(
               $"{nameof(_tabuList)} cannot be empty.");
         }
                                 
         foreach (var item in _tabuList)
         {
            // TODO: Handle expiration
         }
      }

      internal override void OnError()
      {
         base.OnError();
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         // TODO: handle exiting condition with MAX_IMPROVEMENTS AND MAX_ITERATIONS.
         return _currentIteration == _maxImprovements || 
            _currentIteration == _maxIterations || 
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}