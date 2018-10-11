//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 11/10/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;
using System;

namespace CityScover.Engine
{
   internal class NeighborhoodFactory
   {
      internal static Neighborhood CreateNeighborhood(NeighborhoodType type)
      {
         Neighborhood neighborhood = default;

         switch (type)
         {
            case NeighborhoodType.None:
               break;

            case NeighborhoodType.TwoOpt:
               neighborhood = new TwoOptNeighborhood();
               break;

            case NeighborhoodType.CitySwap:
               neighborhood = new CitySwapNeighborhood();
               break;

            case NeighborhoodType.TabuSearchNeighborhood:
               neighborhood = new TabuSearchNeighborhood();
               break;
         }

         return neighborhood;
      }
   }
}