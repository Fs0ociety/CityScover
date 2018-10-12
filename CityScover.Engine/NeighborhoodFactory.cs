//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 12/10/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;
using System;

namespace CityScover.Engine
{
   internal class NeighborhoodFactory
   {
      internal static Neighborhood CreateNeighborhood(AlgorithmType algorithmType)
      {
         Neighborhood neighborhood = default;

         switch (algorithmType)
         {
            case AlgorithmType.None:
               break;

            case AlgorithmType.TwoOpt:
               neighborhood = new TwoOptNeighborhood();
               break;

            case AlgorithmType.CitySwap:
               neighborhood = new CitySwapNeighborhood();
               break;

            case AlgorithmType.TabuSearch:
               neighborhood = new TabuSearchNeighborhood();
               break;

            // Add new Algorithm types here for new Neighborhoods ...

            default:
               break;
         }
         return neighborhood;
      }
   }
}