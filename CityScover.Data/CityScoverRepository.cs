//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 28/07/2018
//

using CityScover.Entities;
using System.Collections.Generic;

namespace CityScover.Data
{
   public static class CityScoverRepository
   {
      private static readonly ICollection<InterestPoint> _points;

      #region Constructors
      static CityScoverRepository()
      {

      }
      #endregion

      #region Private static methods
      // TODO
      #endregion

      #region Public static properties
      public static IEnumerable<InterestPoint> Points => _points;
      #endregion
   }
}
