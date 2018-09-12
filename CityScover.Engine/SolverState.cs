//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 12/09/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using CityScover.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represent the Facade class of the CityScover.Engine, implemented as a Singleton.
   /// Contains the Execute method to run the configuration passed as argument.
   /// The Solver uses ExecutionTracer and SolverHelpers classes to do overall work.
   /// </summary>
   public sealed partial class Solver : Singleton<Solver>
   {
      private ICollection<BaseSolution> _solutions;
      private Lazy<BaseSolution> _bestSolution;
      private Lazy<BlockingCollection<BaseSolution>> _solutionsQueue;
      private Lazy<BlockingCollection<BaseSolution>> _validatingQueue;
      private Lazy<BlockingCollection<BaseSolution>> _evaluatedQueue;
      private ICollection<Task> _solverTasks;

      #region Constructors
      private Solver()
      {
      }
      #endregion

      #region Internal properties
      /// <summary>
      /// Configuration to execute received from the SolverService.
      /// </summary>
      internal Configuration WorkingConfiguration { get; private set; }

      /// <summary>
      /// TODO
      /// </summary>
      internal Problem Problem { get; private set; }

      /// <summary>
      /// Points of interest filtered from the graph of the city.
      /// </summary>
      internal IEnumerable<InterestPoint> Points { get; private set; }

      /// <summary>
      /// Complete graph created which represents the graph of the city.
      /// This object is created by Solver.
      /// </summary>
      internal CityMapGraph CityMapGraph { get; private set; }

      internal BlockingCollection<BaseSolution> SolutionsQueue => _solutionsQueue.Value;
      internal BlockingCollection<BaseSolution> ValidatingQueue => _validatingQueue.Value;
      internal BlockingCollection<BaseSolution> EvaluatedQueue => _evaluatedQueue.Value;

      /// <summary>
      /// All solutions derived from the execution of an Algorithm.
      /// </summary>
      internal IEnumerable<BaseSolution> Solutions { get; private set; }

      internal BaseSolution BestSolution { get; private set; }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         // Lazy initialization: The creation of these objects are deferred until they are first used.
         _solutions = new Collection<BaseSolution>();
         _solutionsQueue = new Lazy<BlockingCollection<BaseSolution>>();
         _validatingQueue = new Lazy<BlockingCollection<BaseSolution>>();
         _evaluatedQueue = new Lazy<BlockingCollection<BaseSolution>>();
         _solverTasks = new Collection<Task>();
      }
      #endregion
   }
}