//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 15/09/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using CityScover.Entities;
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
      private BlockingCollection<TOSolution> _validatingQueue;
      private BlockingCollection<TOSolution> _evaluatedQueue;
      private ICollection<Task> _solverTasks;
      private ICollection<TOSolution> _solutions;
      private TOSolution _bestSolution;

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
      /// This flag says if algorithm's monitoring is active or not.
      /// </summary>
      internal bool IsMonitoringEnabled { get; private set; }

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

      internal BlockingCollection<TOSolution> SolutionsQueue => _solutionsQueue;
      internal BlockingCollection<TOSolution> ValidatingQueue => _validatingQueue;
      internal BlockingCollection<TOSolution> EvaluatedQueue => _evaluatedQueue;

      /// <summary>
      /// All solutions derived from the execution of an Algorithm.
      /// </summary>
      internal IEnumerable<TOSolution> Solutions { get; private set; }

      internal TOSolution BestSolution { get; private set; }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         _solutionsQueue = new BlockingCollection<TOSolution>();
         _validatingQueue = new BlockingCollection<TOSolution>();
         _evaluatedQueue = new BlockingCollection<TOSolution>();
         _solverTasks = new Collection<Task>();
         _solutions = new Collection<TOSolution>();
      }
      #endregion
   }
}