﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 27/11/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;

namespace CityScover.Engine
{
   internal static class NeighborhoodFactory
   {
      #region Internal static methods
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

            case AlgorithmType.TabuSearch:
               //neighborhood = new TabuSearchNeighborhood();
               neighborhood = new TabuSearchNeighborhood2();
               break;

            // Add new Algorithm types here for new Neighborhoods ...
         }
         return neighborhood;
      }
      #endregion
   }
}