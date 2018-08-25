//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 25/08/2018
//

using CityScover.Data;
using CityScover.Engine.Workers;
using CityScover.Utils;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represent the Facade class of the CityScover.Engine, implemented as a Singleton.
   /// Contains the Execute method to run the configuration passed as argument.
   /// The Solver uses ExecutionTracer and SolverHelpers classes to do overall work.
   /// </summary>
   public sealed partial class Solver : Singleton<Solver>
   {
      #region Constructors
      private Solver()
      {
         // Il Solver crea il problema e lo trasmette all'ExecutionTracer.
         //Problem p = new Problem();
      }
      #endregion

      #region Internal properties
      internal Configuration WorkingConfiguration { get; private set; }
      internal CityMapGraph CityMapGraph { get; private set; }
      #endregion

      #region Private methods
      /// <summary>
      /// Initialize the graph of the city using CityScoverRepository.
      /// </summary>
      /// <returns></returns>
      private void CreateCityGraph()
      {
         CityMapGraph cityGraph = new CityMapGraph();
         var points = CityScoverRepository.Points;
         foreach (var point in points)
         {
            cityGraph.AddNode(point.Id, new InterestPointWorker(point));
         }

         var routes = CityScoverRepository.Routes;
         foreach (var route in routes)
         {
            CityMapGraph.AddUndirectEdge(route.PointFrom.Id, route.PointTo.Id, new RouteWorker(route));
         }
         CityMapGraph = cityGraph;
      }
      #endregion
   }
}
