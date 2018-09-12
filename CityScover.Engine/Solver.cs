//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 12/09/2018
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
                           point.Category.Id == TourCategoryType.None
                     select point;
         }

         RoutesGenerator.GenerateRoutes(Points, (ushort)Points.Count());
         CityScoverRepository.LoadRoutes((ushort)Points.Count());

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
         foreach (var solution in _solutionsQueue.GetConsumingEnumerable())
         {
            _validatingQueue.Add(solution);
         }
         _validatingQueue.CompleteAdding();
      }

      /// <summary>
      /// Gets a validated and evaluated Solution from _evaluatedQueue and processes it.
      /// </summary>
      private void TakeEvaluatedSolutions()
      {
         foreach (var solution in _evaluatedQueue.GetConsumingEnumerable())
         {
            _solutions.Add(solution);
         }
      }

      /// <summary>
      /// Run the algorithm directly without monitoring process.
      /// </summary>
      /// <param name="configuration"></param>
      /// <returns></returns>
      private async Task ExecuteWithoutMonitoring()
      {
         bool exceptionOccurred = false;

         // Run all Stages.
         foreach (Stage stage in WorkingConfiguration.Stages)
         {
            Algorithm algorithm = AlgorithmFactory.CreateAlgorithm(stage.CurrentAlgorithm);

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
      }

      /// <summary>
      /// Run the algorithm indirectly from the ExecutionReporter with monitoring of the first.
      /// </summary>
      /// <param name="configuration"></param>
      /// <returns></returns>
      private async Task ExecuteWithMonitoring()
      {
         bool exceptionOccurred = false;

         // Run all Stages.
         foreach (Stage stage in WorkingConfiguration.Stages)
         {
            Algorithm algorithm = AlgorithmFactory.CreateAlgorithm(stage.CurrentAlgorithm);

            if (algorithm == null)
            {
               throw new NullReferenceException(nameof(algorithm));
            }

            algorithm.Provider = new AlgorithmTracker() ?? throw new NullReferenceException();
            ExecutionReporter reporter = new ExecutionReporter();
            reporter.Subscribe(algorithm.Provider);

            try
            {
               await Task.Run(() => reporter.Run(algorithm));
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
      }

      private BaseSolution GetBestSolution()
      {
         // TODO: Uses LINQ query.
         throw new NotImplementedException();
      }
      #endregion

      #region Public methods
      /// <summary>
      /// Executes configuration passed as parameter while loop around stages.
      /// </summary>
      /// <param name="configuration"></param>
      public async Task Execute(Configuration configuration, bool enableMonitoring = false)
      {
         WorkingConfiguration = configuration;
         Problem = ProblemFactory.CreateProblem(configuration.CurrentProblem);
         InitializeTour();
         RunWorkers();

         void RunWorkers()
         {
            _solverTasks.Add(Task.Run(() => TakeNewSolutions()));
            _solverTasks.Add(Task.Run(() => TakeEvaluatedSolutions()));
            _solverTasks.Add(Task.Run(() => SolverValidator.Instance.Run()));
            _solverTasks.Add(Task.Run(() => SolverEvaluator.Instance.Run()));
         }

         if (enableMonitoring)
         {
            await ExecuteWithMonitoring();
         }
         else
         {
            await ExecuteWithoutMonitoring();
         }

         _solutionsQueue.CompleteAdding();
         await Task.WhenAll(_solverTasks.ToArray());

         // TODO: Loop in solutions collection to select best solution.
         BestSolution = GetBestSolution();

         throw new NotImplementedException(nameof(Execute));
      }
      #endregion
   }
}
