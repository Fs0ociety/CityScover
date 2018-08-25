//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 25/08/2018
//

using CityScover.Data;
using CityScover.Engine.Workers;
using CityScover.Entities;
using CityScover.Utils;
using System;
using System.Linq;

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

      private void FilterPointsByCategory()
      {
         Points = from point in CityScoverRepository.Points
                  where point.Category.Id == WorkingConfiguration.TourCategory ||
                        point.Category.Id == TourCategoryType.None
                  select point;
      }
      #endregion

      #region Public methods
      public void Initialize()
      {
         CityScoverRepository.LoadPoints(WorkingConfiguration.PointsCount);
         FilterPointsByCategory();

         RoutesGenerator.GenerateRoutes(Points, (ushort)Points.Count());
         CityScoverRepository.LoadRoutes((ushort)Points.Count());

         CreateCityGraph();
      }

      public void Execute(Configuration configuration)
      {
         SetWorkingConfig();
         Initialize();

         void SetWorkingConfig()
         {
            WorkingConfiguration = configuration;
            SolverConstraintHelper.Instance.WorkingConfiguration = configuration;
            SolverEvaluationHelper.Instance.WorkingConfiguration = configuration;
         }

         throw new NotImplementedException(nameof(Execute));
      }
      #endregion
   }
}
