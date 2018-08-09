//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 30/07/2018
//

using CityScover.Data;
using CityScover.Engine;
using CityScover.Entities;
using System.Collections.Generic;

namespace CityScover
{
   class Program
   {
      static void Main(string[] args)
      {
         IEnumerable<InterestPoint> points = CityScoverRepository.Points;
         RoutesGenerator.GenerateRoutes((ICollection<InterestPoint>)points);
         IEnumerable<Route> routes = CityScoverRepository.Routes;

         Solver.Instance.Run();
      }
   }
}
