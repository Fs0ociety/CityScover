//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using CityScover.Data;
using CityScover.Entities;
using CityScover.Utils;
using System.Collections.Generic;

namespace CityScover.Services
{
   public class TourService : Singleton<TourService>, ITourService
   {
      public IEnumerable<InterestPoint> Points => CityScoverRepository.Points;

      public IEnumerable<Route> Routes
      {
         get
         {
            RoutesGenerator.GenerateRoutes((ICollection<InterestPoint>)Points);
            return CityScoverRepository.Routes;
         }
      }

      public Tour CreateTour()
      {
         // TODO
         throw new System.NotImplementedException();
      }
   }
}
