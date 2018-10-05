//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 05/10/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   internal class TabuSearch : Algorithm
   {
      private IEnumerable<Algorithm> _childrenAlgorithms;
      private IList<TOSolution> _tabuList;
      private TOSolution _bestSolution;
      private Neighborhood _neighborhood;

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
      #endregion

      #region Overrides
      internal override void OnError()
      {
         base.OnError();
      }

      internal override void OnInitializing()
      {
         base.OnInitializing();
         _bestSolution = Solver.BestSolution;
         _tabuList = new List<TOSolution>();
         _childrenAlgorithms = GetInnerAlgorithms();

         if (_childrenAlgorithms == null)
         {
            throw new InvalidOperationException(
               $"Bad configuration format: {nameof(Solver.WorkingConfiguration)}.");
         }
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override async Task PerformStep()
      {
         foreach (var algorithm in _childrenAlgorithms)
         {
            await Task.Run(() => algorithm.Start());

            // ADESSO COME CAZZO ACCEDO ALL'INTORNO DELLE SOLUZIONI?
         }
      }

      internal override bool StopConditions()
      {
         // TODO: handle exiting condition with MAX_IMPROVEMENTS AND MAX_ITERATIONS.
         return _status == AlgorithmStatus.Error;
      }
      #endregion
   }
}
