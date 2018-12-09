//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/12/2018
//

namespace CityScover.Engine.Algorithms.Neighborhoods
{
   internal static class NeighborhoodFactory
   {
      #region Internal static methods
      internal static Neighborhood<ToSolution> CreateNeighborhood(AlgorithmType algorithmType)
      {
         Neighborhood<ToSolution> neighborhood = default;

         switch (algorithmType)
         {
            case AlgorithmType.None:
               break;

            case AlgorithmType.TwoOpt:
            case AlgorithmType.TabuSearch:
               neighborhood = new TwoOptNeighborhood();
               break;

            // Add new Algorithm types here for new Neighborhoods ...
         }
         return neighborhood;
      }
      #endregion
   }
}
