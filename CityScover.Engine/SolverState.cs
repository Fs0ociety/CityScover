//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 07/11/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using CityScover.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
      #region Private fields
      private BlockingCollection<TOSolution> _solutionsQueue;
      private ICollection<Task> _solverTasks;
      private IDictionary<int, Task> _algorithmTasks;
      #endregion

      #region Constructors
      private Solver()
      {
      }
      #endregion

      #region Private properties
      private SolverValidator SolverValidator => SolverValidator.Instance;
      private SolverEvaluator SolverEvaluator => SolverEvaluator.Instance; 
      #endregion

      #region Internal properties
      /// <summary>
      /// Configuration to execute received from the SolverService.
      /// </summary>
      internal Configuration WorkingConfiguration { get; private set; }

      /// <summary>
      /// This flag says if algorithm's monitoring is active or not.
      /// </summary>
      internal bool IsMonitoringEnabled { get; private set; }

      /// <summary>
      /// This delegate contains execution method to invoke by the current Algorithm.
      /// </summary>
      internal Func<Algorithm, Task> ExecutionInternalFunc { get; private set; }

      /// <summary>
      /// Current stage in execution.
      /// </summary>
      internal Stage CurrentStage { get; private set; }

      /// <summary>
      /// Points of interest filtered from the graph of the city.
      /// </summary>
      internal IEnumerable<InterestPoint> Points { get; private set; }

      /// <summary>
      /// Current type of Operating Reasearch problem.
      /// </summary>
      internal ProblemBase Problem { get; private set; }

      /// <summary>
      /// Instance size of the current problem.
      /// </summary>
      internal int ProblemSize => Points.Count();

      /// <summary>
      /// Current Graph representing the map of the city.
      /// </summary>
      internal CityMapGraph CityMapGraph { get; private set; }
      
      internal TOSolution BestSolution { get; set; }

      internal int PreviousStageSolutionCost { get; set; }

      internal IDictionary<int, Task> AlgorithmTasks => _algorithmTasks;
      
      internal ICollection<Result> Results { get; private set; }

      internal ICollection<string> ConstraintsToRelax { get; private set; }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         _solverTasks = new Collection<Task>();
         _algorithmTasks = new Dictionary<int, Task>();
         Results = new Collection<Result>();
         ConstraintsToRelax = new Collection<string>();
      }
      #endregion
   }
}