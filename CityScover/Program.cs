//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using CityScover.Engine;
using CityScover.Services;

namespace CityScover
{
   class Program
   {
      static void Main(string[] args)
      {
         InitializeRepository();
         Solver.Instance.Run();
      }

      private static void InitializeRepository()
      {
         ITourService tourService = TourService.Instance;
         var points = tourService.Points;
         var routes = tourService.Routes;
      }
   }
}
