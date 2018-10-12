//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 12/10/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal class TabuSearch : Algorithm
   {
      private LocalSearch _innerAlgorithm;
      private TabuSearchNeighborhood _neighborhood;
      private TOSolution _bestSolution;
      private int _currentIteration;
      private int _maxIterations;
      private int _maxImprovements;    // L'idea è di gestire maxImprovements nello StageFlow come fatto per il RunningCount
      private int _tenure;

      #region Constructors
      internal TabuSearch()
         : this(null)
      {
      }

      internal TabuSearch(Neighborhood neighborhood)
         : this(neighborhood, null)
      {
      }

      internal TabuSearch(Neighborhood neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
         if (neighborhood is TabuSearchNeighborhood tbNeighborhood)
         {
            _neighborhood = tbNeighborhood;
         }
      }
      #endregion

      #region Private methods

      #region Temporary code for VNS algorithm
      //private IEnumerable<Algorithm> GetInnerAlgorithms()
      //{
      //   var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
      //   if (childrenAlgorithms == null)
      //   {
      //      return null;
      //   }

      //   ICollection<Algorithm> algorithms = new Collection<Algorithm>();
      //   foreach (var children in childrenAlgorithms)
      //   {
      //      algorithms.Add(Solver.GetAlgorithm(children.CurrentAlgorithm));
      //   }

      //   return algorithms;
      //}
      #endregion

      private LocalSearch GetLocalSearchAlgorithm()
      {
         var childrenAlgorithms = Solver.CurrentStage.Flow.ChildrenFlows;
         if (childrenAlgorithms == null)
         {
            return null;
         }

         var flow = childrenAlgorithms.FirstOrDefault();
         if (flow == null)
         {
            return null;
         }

         _neighborhood.NeighborhoodWorker = NeighborhoodFactory.CreateNeighborhood(flow.CurrentAlgorithm);
         Algorithm algorithm = Solver.GetAlgorithm(flow.CurrentAlgorithm, _neighborhood);

         if (algorithm is LocalSearch ls)
         {
            _innerAlgorithm = ls;
            _maxIterations = flow.RunningCount;
         }

         return _innerAlgorithm;
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
         //_maxImprovements = _maxIterations;   // Gestire diversamente
         _bestSolution = Solver.BestSolution;
         _innerAlgorithm = GetLocalSearchAlgorithm();

         if (_innerAlgorithm == null)
         {
            throw new InvalidOperationException($"Bad configuration format: " +
               $"{nameof(Solver.WorkingConfiguration)}.");
         }

         _neighborhood.TabuList = new List<TabuMove>();
         _innerAlgorithm.AcceptImprovementsOnly = false;
         _innerAlgorithm.Provider = Provider;
      }

      internal override async Task PerformStep()
      {
         await _innerAlgorithm.Start();

         // TODO: Eliminare il MAX IMPROVEMENTS E USARE LA CONDIZIONE DI STOP SULLO STALLO
         // (Stallo = no improvement per almeno k iterazioni)

         foreach (var move in _neighborhood.TabuList)
         {
            if (move.Expiration >= _neighborhood.TabuList.Count)   // Gestire con il "Tenure" e non il Count della Tabu list
            {
               // Aspiration Criteria
               _neighborhood.TabuList.Remove(move);
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