//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/09/2018
//

using CityScover.Commons;
using CityScover.Data;
using CityScover.Engine.Workers;
using CityScover.Entities;
using System;
using System.Diagnostics;
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
      #region Private methods
      /// <summary>
      /// Initialize the working configuration and it creates the Problem.
      /// </summary>
      /// <param name="configuration"></param>
      private void InitSolver(Configuration configuration)
      {
         WorkingConfiguration = configuration;
         IsMonitoringEnabled = configuration.AlgorithmMonitoring;
         Problem = ProblemFactory.CreateProblem(configuration.CurrentProblem);

         if (Problem == null)
         {
            throw new NullReferenceException(nameof(Problem));
         }
      }

      /// <summary>
      /// Initialize the graph of the city using CityScoverRepository.
      /// </summary>
      private void InitializeTour()
      {
         CityScoverRepository.LoadPoints(WorkingConfiguration.PointsCount);
         FilterPointsByCategory();

         void FilterPointsByCategory()
         {
            Points = from point in CityScoverRepository.Points
                     where point.Category.Id == WorkingConfiguration.TourCategory ||
                           point.Category.Id == TourCategoryType.None   // Hotel
                     select point;
         }

         RoutesGenerator.GenerateRoutes(Points, Points.Count());
         CityScoverRepository.LoadRoutes(Points);

         CreateCityGraph();

         void CreateCityGraph()
         {
            CityMapGraph cityGraph = new CityMapGraph();
            foreach (var point in Points)
            {
               cityGraph.AddNode(point.Id, new InterestPointWorker(point));
            }

            var routes = CityScoverRepository.Routes;
            foreach (var route in routes)
            {
               cityGraph.AddEdge(route.PointFrom.Id, route.PointTo.Id, new RouteWorker(route));
            }

            CityMapGraph = cityGraph;
         }
      }

      /// <summary>
      /// Gets a new Solution from the _solutionsQueue and processes it.
      /// </summary>
      private void TakeNewSolutions()
      {
         foreach (var solution in SolutionsQueue.GetConsumingEnumerable())
         {
            ValidatingQueue.Add(solution);
         }
         ValidatingQueue.CompleteAdding();
      }

      /// <summary>
      /// Gets a validated and evaluated Solution from _evaluatedQueue and processes it.
      /// </summary>
      private void TakeEvaluatedSolutions()
      {
         foreach (var solution in EvaluatedQueue.GetConsumingEnumerable())
         {
            _solutions.Add(solution);
         }
      }

      /// <summary>
      /// Returns the best solution produced from an Algorithm.
      /// </summary>
      /// <returns></returns>
      private TOSolution GetBestSolution()
      {
         var isMinimizingProblem = Problem.IsMinimizing;
         var costs = from sol in _solutions
                     select sol.Cost;

         var targetCost = isMinimizingProblem ? costs.Min() : costs.Max();

         return (from solution in _solutions
                 where solution.Cost == targetCost
                 select solution).FirstOrDefault();
      }

      /// <summary>
      /// Used from an Algorithm that must execute an internal algorithm with monitoring disabled.
      /// </summary>
      /// <param name="algorithm">Algorithm to execute.</param>
      /// <returns></returns>
      private async Task ExecuteWithoutMonitoring(Algorithm algorithm)
      {
         bool exceptionOccurred = false;

         if (algorithm == null)
         {
            throw new NullReferenceException(nameof(algorithm));
         }

         try
         {
            await Task.Run(() => algorithm.Start());
         }
         catch (Exception ex)
         {
            exceptionOccurred = true;
            Debug.WriteLine(GetType().Name);
            Debug.WriteLine(ex.Message);
         }

         if (!exceptionOccurred)
         {
            // TODO
         }
      }

      /// <summary>
      /// Used from an Algorithm that must execute an internal algorithm with monitoring enabled.
      /// </summary>
      /// <param name="algorithm">Algorithm to execute.</param>
      /// <returns></returns>
      private async Task ExecuteWithMonitoring(Algorithm algorithm)
      {
         bool exceptionOccurred = false;

         if (algorithm == null)
         {
            throw new NullReferenceException(nameof(algorithm));
         }

         algorithm.Provider = new AlgorithmTracker() ?? throw new NullReferenceException();
         ExecutionReporter reporter = new ExecutionReporter();
         reporter.Subscribe(algorithm.Provider);

         try
         {
            await reporter.Run(algorithm);
         }
         catch (Exception ex)
         {
            exceptionOccurred = true;
            Debug.WriteLine(GetType().Name);
            Debug.WriteLine(ex.Message);
         }
         finally
         {
            reporter.Unsubscribe();
         }

         if (!exceptionOccurred)
         {
            // TODO
         }
      }

      /// <summary>
      /// Launch a Configuration, running all its stages.
      /// </summary>
      /// <param name="executionFunc">Method to invoke.</param>
      /// <returns></returns>
      private async Task ExecuteConfiguration(Func<Algorithm, Task> executionFunc)
      {
         // Run all Stages.
         foreach (Stage stage in WorkingConfiguration.Stages)
         {
            CurrentStage = stage;
            Algorithm algorithm = AlgorithmFactory.CreateAlgorithm(stage.Flow.CurrentAlgorithm);
            await executionFunc.Invoke(algorithm);
         }
      }
      #endregion

      #region Internal methods
      internal Algorithm GetAlgorithm(AlgorithmType algorithmType) =>
         AlgorithmFactory.CreateAlgorithm(algorithmType);
      #endregion

      #region Public methods
      /// <summary>
      /// Executes configuration passed as parameter while loop around stages.
      /// </summary>
      /// <param name="configuration">Configuration to launch.</param>
      public async Task Execute(Configuration configuration)
      {
         InitSolver(configuration);
         InitializeTour();
         //RunWorkers();

         void RunWorkers()
         {
            _solverTasks.Add(Task.Run(() => TakeNewSolutions()));
            _solverTasks.Add(Task.Run(() => TakeEvaluatedSolutions()));
            _solverTasks.Add(Task.Run(() => SolverValidator.Instance.Run()));
            _solverTasks.Add(Task.Run(() => SolverEvaluator.Instance.Run()));
         }
      
         if (IsMonitoringEnabled)
         {
            ExecutionInternalFunc = ExecuteWithMonitoring;
            await ExecuteConfiguration(ExecuteWithMonitoring);
         }
         else
         {
            ExecutionInternalFunc = ExecuteWithoutMonitoring;
            await ExecuteConfiguration(ExecuteWithoutMonitoring);
         }

         SolutionsQueue.CompleteAdding();
         await Task.WhenAll(_solverTasks.ToArray());
         BestSolution = GetBestSolution();

         throw new NotImplementedException(nameof(Execute));
      }
      #endregion
   }
}