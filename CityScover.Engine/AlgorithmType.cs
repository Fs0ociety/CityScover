//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/08/2018
//

namespace CityScover.Engine
{
   public enum AlgorithmType
   {
      /// <summary>
      /// Invalid algorithm.
      /// </summary>
      None = 0,

      /// <summary>
      /// NearestNeighbor algorithm 
      /// with classic closest neighbor best function enumerator.
      /// </summary>
      NearestNeighbor = 1,
      
      /// <summary>
      /// NearestNeighbor algorithm 
      /// with knapsack best function enumerator.
      /// </summary>
      NearestNeighborKnapsack = 2,

      /// <summary>
      /// NearestInsertion algorithm enumerator.
      /// </summary>
      NearestInsertion = 3,

      /// <summary>
      /// CheapestInsertion algorithm enumerator.
      /// </summary>
      CheapestInsertion = 4,

      /// <summary>
      /// TwoOpt algorithm enumerator.
      /// </summary>
      TwoOpt = 5,

      /// <summary>
      /// CitySwap algorithm enumerator.
      /// </summary>
      CitySwap = 6,

      /// <summary>
      /// LinKernighan algorithm enumerator.
      /// </summary>
      LinKernighan = 7,

      /// <summary>
      /// IteratedLocalSearch algorithm enumerator.
      /// </summary>
      IteratedLocalSearch = 8,

      /// <summary>
      /// TabuSearch algorithm enumerator.
      /// </summary>
      TabuSearch = 9,

      /// <summary>
      /// VariableNeighborhoodSearch algorithm enumerator.
      /// </summary>
      VariableNeighborhoodSearch = 10
   }
}