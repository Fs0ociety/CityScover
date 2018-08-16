//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using CityScover.Entities;
using System.Collections.Generic;

namespace CityScover.Services
{
   public interface ITourService
   {
      Tour CreateTour();
      IEnumerable<InterestPoint> Points { get; }

      IEnumerable<Route> Routes { get; }
   }
}
