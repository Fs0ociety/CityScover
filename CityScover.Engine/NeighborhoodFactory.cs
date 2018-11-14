//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 14/11/2018
//

using CityScover.Engine.Algorithms.Neighborhoods;

namespace CityScover.Engine
{
   internal class NeighborhoodFactory
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
               neighborhood = new TabuSearchNeighborhood();
               break;

            // Add new Algorithm types here for new Neighborhoods ...

            default:
               break;
         }
         return neighborhood;
      }
      #endregion
   }
}