//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 26/09/2018
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
      private BlockingCollection<TOSolution> _solutionsQueue;
      private BlockingCollection<TOSolution> _processedSolutions;
      private ICollection<Task> _solverTasks;
      
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
      
      internal TOSolution BestSolution { get; set; }

      internal IEnumerable<Result> InvalidResults { get; set; }

      internal IEnumerable<Result> ValidResults { get; set; }      
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         _solutionsQueue = new BlockingCollection<TOSolution>();         
         _solverTasks = new Collection<Task>();
         _processedSolutions = new BlockingCollection<TOSolution>();
         InvalidResults = new Collection<Result>();
         ValidResults = new Collection<Result>();
      }
      #endregion
   }
}