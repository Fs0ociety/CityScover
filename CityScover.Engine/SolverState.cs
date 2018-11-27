//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 25/11/2018
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
      private BlockingCollection<ToSolution> _solutionsQueue;
      private ICollection<Task> _solverTasks;
      
      /// <summary>
      /// This delegate contains execution method to invoke by the current Algorithm.
      /// </summary>
      private Func<Algorithm, Task> _executionFunc;
      #endregion

      #region Constructors
      private Solver()
      {
      }
      #endregion

      #region Private properties
      private SolverValidator SolverValidator => SolverValidator.Instance;
      private SolverEvaluator SolverEvaluator => SolverEvaluator.Instance;

      /// <summary>
      /// Points of interest filtered from the graph of the city.
      /// </summary>
      private IEnumerable<InterestPoint> Points { get; set; }
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
      /// Current stage in execution.
      /// </summary>
      internal Stage CurrentStage { get; private set; }

      /// <summary>
      /// The running time of the stage in execution.
      /// </summary>
      internal TimeSpan CurrentStageExecutionTime { get; set; }
      /// <summary>
      /// Current algorithm in execution.
      /// </summary>
      internal AlgorithmType CurrentAlgorithm { get; set; }

      /// <summary>
      /// Current objective function weight.
      /// </summary>
      internal double CurrentObjectiveFunctionWeight { get; set; }

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
      
      internal ToSolution BestSolution { get; set; }

      internal int PreviousStageSolutionCost { get; set; }

      internal IDictionary<int, Task> AlgorithmTasks { get; private set; }

      internal ICollection<string> ConstraintsToRelax { get; private set; }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         _solverTasks = new Collection<Task>();
         AlgorithmTasks = new Dictionary<int, Task>();
         ConstraintsToRelax = new Collection<string>();
      }
      #endregion
   }
}