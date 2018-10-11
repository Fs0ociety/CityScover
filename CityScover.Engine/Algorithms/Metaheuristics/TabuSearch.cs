﻿//
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
      private Algorithm _localSearchAlgorithm;
      private byte _localSearchRunningCount;
      private IList<RouteWorker> _tabuList;     // Capire come sarà formata al suo interno la Tabu List con il parametro expiration.
      private TOSolution _bestSolution;
      private int _currentIteration;
      private int _maxImprovements;    // L'idea è di gestire maxImprovements nello StageFlow come fatto per il RunningCount
      private int _maxIterations;      // L'idea è di gestire maxIteration nello StageFlow come fatto per il RunningCount

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

      private Algorithm GetLocalSearchAlgorithm()
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
         _localSearchRunningCount = algorithmFlow.RunningCount;

         switch (algorithmFlow.CurrentAlgorithm)
         {
            case AlgorithmType.TwoOpt:
               _localSearchAlgorithm = new LocalSearch<RouteWorker, IEnumerable<RouteWorker>>(new TabuSearchNeighborhood<RouteWorker, IEnumerable<RouteWorker>>(
                  new TwoOptNeighborhood()), Provider);
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
         _maxIterations = 2;
         _maxImprovements = _maxIterations;
         _bestSolution = Solver.BestSolution;
         _tabuList = new List<RouteWorker>();
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
         if (_localSearchRunningCount > 0)
         {
            await _localSearchAlgorithm.Start();

            //if (!_tabuList.Any())
            //{
            //   throw new InvalidOperationException(
            //      $"{nameof(_tabuList)} cannot be empty.");
            //}

            foreach (var item in _tabuList)
            {
               // TODO: Handle expiration criteria.
            }

            _localSearchRunningCount--;
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
         // TODO: handle exiting condition with MAX_IMPROVEMENTS AND MAX_ITERATIONS.
         return _currentIteration == _maxImprovements ||
            _currentIteration == _maxIterations ||
            _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}