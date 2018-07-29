using CityScover.Data;
using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CityScover
{
   class Program
   {
      static void Main(string[] args)
      {
         IEnumerable<InterestPoint> points = CityScoverRepository.Points;
      }
   }
}
