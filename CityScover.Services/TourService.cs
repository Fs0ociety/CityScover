//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

using CityScover.Entities;
using CityScover.Utils;

namespace CityScover.Services
{
   public class TourService : Singleton<TourService>, ITourService
   {
      #region ITourService implementation
      public Tour CreateTour()
      {
         throw new System.NotImplementedException();
      } 
      #endregion
   }
}
