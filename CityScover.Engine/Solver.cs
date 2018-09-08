//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 08/09/2018
//

using CityScover.Data;
using CityScover.Engine.Algorithms.Greedy;
using CityScover.Engine.Workers;
using CityScover.Entities;
using CityScover.Utils;
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
      /// <returns></returns>
      private void CreateCityGraph()
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

      private void Initialize()
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
      }

      private (Algorithm algorithm, AlgorithmTracker provider) CreateAlgorithm(AlgorithmType currentAlgorithm)
      {
         Algorithm algorithm = default;
         AlgorithmTracker provider = new AlgorithmTracker();

         switch (currentAlgorithm)
         {
            case AlgorithmType.NearestNeighbor:
               algorithm = new NearestNeighborAlgorithm(provider);
               break;

            case AlgorithmType.NearestInsertion:
               break;

            case AlgorithmType.CheapestInsertion:
               break;

            case AlgorithmType.TwoOpt:
               break;

            case AlgorithmType.CitySwap:
               break;

            case AlgorithmType.LinKernighan:
               break;

            case AlgorithmType.IteratedLocalSearch:
               break;

            case AlgorithmType.TabuSearch:
               break;

            case AlgorithmType.VariableNeighborhoodSearch:
               break;

            default:
               provider = null;
               break;
         }

         return (algorithm, provider);
      }
      #endregion

      #region Public methods
      /// <summary>
      /// TODO
      /// </summary>
      /// <param name="configuration"></param>
      public async Task Execute(Configuration configuration)
      {
         WorkingConfiguration = configuration;
         Initialize();

         bool exceptionOccurred = false;

         // Run all Stages.
         foreach (Stage stage in configuration.Stages)
         {
            (Algorithm algorithm, AlgorithmTracker provider) = CreateAlgorithm(stage.CurrentAlgorithm);

            if (algorithm == null)
            {
               throw new NullReferenceException(nameof(algorithm));
            }

            if (provider == null)
            {
               throw new NullReferenceException(nameof(provider));
            }

            ExecutionReporter reporter = new ExecutionReporter();
            reporter.Subscribe(provider);

            try
            {
               Task executionTask = Task.Run(() => reporter.Run(algorithm));
               await executionTask;
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

         _solutionsQueue.CompleteAdding();
         await Task.WhenAll(_solverTasks.ToArray());
         throw new NotImplementedException(nameof(Execute));
      }
      #endregion
   }
}
