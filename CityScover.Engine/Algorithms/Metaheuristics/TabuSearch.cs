//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/10/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Workers;
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
      private LocalSearch _innerAlgorithm;
      private TOSolution _bestSolution;
      private IList<TabuMove> _tabuList;
      private int _currentIteration;
      private int _maxIterations;
      private int _maxImprovements;    // L'idea è di gestire maxImprovements nello StageFlow come fatto per il RunningCount

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
         _maxIterations = algorithmFlow.RunningCount;

         switch (algorithmFlow.CurrentAlgorithm)
         {
            case AlgorithmType.TwoOpt:
               _innerAlgorithm = new LocalSearch(new TabuSearchNeighborhood(
                  new TwoOptNeighborhood(), _tabuList), Provider);
               break;

            case AlgorithmType.CitySwap:
               // TODO: Create CitySwap neighborhood object
               break;

            // Repeat for other Local Search algorithms...

            default:
               _innerAlgorithm = null;
               break;
         }

         return _innerAlgorithm;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

         _currentIteration = default;
         _maxImprovements = _maxIterations;
         _bestSolution = Solver.BestSolution;
         _tabuList = new List<TabuMove>();
         _innerAlgorithm = GetLocalSearchAlgorithm();

         if (_innerAlgorithm == null)
         {
            throw new InvalidOperationException($"Bad configuration format: " +
               $"{nameof(Solver.WorkingConfiguration)}.");
         }

         _innerAlgorithm.AcceptImprovementsOnly = false;
      }

      internal override async Task PerformStep()
      {
         await _innerAlgorithm.Start();

         if (!_tabuList.Any())
         {
            throw new InvalidOperationException(
               $"{nameof(_tabuList)} cannot be empty.");
         }

         foreach (var move in _tabuList)
         {
            if (move.Expiration >= _tabuList.Count)
            {
               // Aspiration Criteria
               _tabuList.Remove(move);
            }
         }
         _currentIteration++;
      }

      internal override void OnError()
      {
         base.OnError();
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
         Solver.BestSolution = _bestSolution;
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return _currentIteration == _maxImprovements ||
            _currentIteration == _maxIterations ||
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}