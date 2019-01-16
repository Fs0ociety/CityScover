//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 22/12/2018
//

using CityScover.Data;
using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Workers;
using CityScover.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
   public sealed partial class Solver
   {
      #region Private methods
      /// <summary>
      /// Creates the problem contained in the configuration 
      /// and initializes the working configuration.
      /// </summary>
      /// <param name="configuration"></param>
      private void InitSolver(Configuration configuration)
      {
         if (_solutionsQueue is null)
         {
            _solutionsQueue = new BlockingCollection<ToSolution>();
         }

         ToSolution.ResetSequenceId();
         WorkingConfiguration = configuration;
         IsMonitoringEnabled = configuration.AlgorithmMonitoring;
         Problem = ProblemFactory.CreateProblem(configuration.CurrentProblem);
      }

      /// <summary>
      /// Initialize the graph of the city using CityScoverRepository.
      /// </summary>
      private void InitializeTour()
      {
         CityScoverRepository.LoadPoints(WorkingConfiguration.PointsFilename);

         Points = (WorkingConfiguration.TourCategory == TourCategoryType.None) ? 
            CityScoverRepository.Points : 
            GetPointsByCategory(WorkingConfiguration.TourCategory);

         RoutesGenerator.GenerateRoutes(Points, Points.Count());
         CityScoverRepository.LoadRoutes(Points);

         // Creation of the Graph.
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

         if (cityGraph.NodeCount == 0)
         {
            string message = MessagesRepository.GetMessage(
                MessageCode.SolverGraphCreationError, nameof(InitializeTour));
            throw new Exception(message);
         }

         CityMapGraph = cityGraph;
      }

      private IEnumerable<InterestPoint> GetPointsByCategory(TourCategoryType category) => 
         CityScoverRepository.Points.Where(point => point.Category.Id == category || 
                                                    point.Category.Id == TourCategoryType.None);

      /// <summary>
      /// Gets a new Solution from the _solutionsQueue and processes it.
      /// </summary>
      private void TakeNewSolutions()
      {
         foreach (var solution in _solutionsQueue.GetConsumingEnumerable())
         {
            Task processingSolutionTask = Task.Run(() => ProcessSolution(solution));
            AlgorithmTasks.Add(solution.Id, processingSolutionTask);
         }

         void ProcessSolution(ToSolution solution)
         {
            SolverValidator.Validate(solution);
            SolverEvaluator.Evaluate(solution);
         }
      }

      /// <summary>
      /// Used from an Algorithm that must execute an internal algorithm with monitoring disabled.
      /// </summary>
      /// <param name="algorithm">Algorithm to execute.</param>
      /// <returns></returns>
      private async Task ExecuteWithoutMonitoring(Algorithm algorithm)
      {
         try
         {
            await Task.Run(algorithm.Start);
         }
         catch (Exception ex)
         {
            Debug.WriteLine(GetType().Name);
            Debug.WriteLine(ex.Message);
         }
      }

      /// <summary>
      /// Used from an Algorithm that must execute an internal algorithm with monitoring enabled.
      /// </summary>
      /// <param name="algorithm">Algorithm to execute.</param>
      /// <returns></returns>
      private async Task ExecuteWithMonitoring(Algorithm algorithm)
      {
         algorithm.Provider = new AlgorithmTracker();
         ExecutionReporter reporter = new ExecutionReporter();
         reporter.Subscribe(algorithm.Provider);

         try
         {
            await reporter.Run(algorithm);
         }
         catch (Exception ex)
         {
            Debug.WriteLine(GetType().Name);
            Debug.WriteLine(ex.Message);
         }
         finally
         {
            reporter.Unsubscribe();
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
            Console.WriteLine(MessagesRepository.GetMessage(MessageCode.SolverStageStart, CurrentStage.Description));
            Algorithm algorithm = GetAlgorithm(stage.Flow.CurrentAlgorithm);
            if (algorithm is null)
            {
               throw new NullReferenceException(nameof(algorithm));
            }
            CurrentAlgorithm = algorithm.Type;
            algorithm.Parameters = stage.Flow.AlgorithmParameters;
            await executionFunc.Invoke(algorithm);
            CurrentStageExecutionTime = default;
            Console.WriteLine(MessagesRepository.GetMessage(MessageCode.SolverStageEnd, CurrentStage.Description));
         }

         Console.WriteLine(MessagesRepository.GetMessage(
            MessageCode.SolverEndConfiguration, BestSolution.Id, BestSolution.Cost, 
            BestSolution.Tour.GetTotalDistance() * 0.001));
      }

      private async Task ResetSolverState()
      {
         _solutionsQueue.CompleteAdding();
         await Task.WhenAll(_solverTasks);
         _solutionsQueue.Dispose();
         _solutionsQueue = default;
         _executionFunc = default;
         Points = default;
         Problem = default;
         CityMapGraph = default;
         BestSolution = default;
         CurrentStage = default;
         IsMonitoringEnabled = default;
         WorkingConfiguration = default;
         PreviousStageSolutionCost = default;
         CurrentStageExecutionTime = default;
         CurrentAlgorithm = AlgorithmType.None;
         _solverTasks.Clear();
         AlgorithmTasks.Clear();
         ConstraintsToRelax.Clear();
      }
      #endregion

      #region Internal methods
      internal Algorithm GetAlgorithm(AlgorithmType algorithmType) =>
         AlgorithmFactory.CreateAlgorithm(algorithmType);

      internal Algorithm GetAlgorithm(AlgorithmType algorithmType, Neighborhood<ToSolution> neighborhood) =>
         AlgorithmFactory.CreateAlgorithm(algorithmType, neighborhood);

      internal IEnumerable<Algorithm> GetImprovementAlgorithms(IEnumerable<StageFlow> childrenFlows) =>
         AlgorithmFactory.GetImprovementAlgorithms(childrenFlows);

      internal void EnqueueSolution(ToSolution solution) =>
         _solutionsQueue.Add(solution);

      internal void InitConstraintsToValidate() => SolverValidator.InitializeProblemConstraints();
      #endregion

      #region Public methods
      /// <summary>
      /// Executes configuration passed as parameter while loop around stages.
      /// </summary>
      /// <param name="configuration">Configuration to launch.</param>
      public async Task Execute(Configuration configuration)
      {
         InitSolver(configuration);
         try
         {
            InitializeTour();
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex.Message);
            return;
         }
         _solverTasks.Add(Task.Run(TakeNewSolutions));

         if (IsMonitoringEnabled)
         {
            //ExecutionInternalFunc = ExecuteWithMonitoring;
            _executionFunc = ExecuteWithMonitoring;
            //await ExecuteConfiguration(ExecuteWithMonitoring);
         }
         else
         {
            //ExecutionInternalFunc = ExecuteWithoutMonitoring;
            _executionFunc = ExecuteWithoutMonitoring;
            //await ExecuteConfiguration(ExecuteWithoutMonitoring);
         }

         await ExecuteConfiguration(_executionFunc);
         await ResetSolverState();
      }
      #endregion
   }
}