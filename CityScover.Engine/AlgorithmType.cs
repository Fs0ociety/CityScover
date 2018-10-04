//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 30/09/2018
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
      /// CheapestInsertion algorithm enumerator.
      /// </summary>
      CheapestInsertion = 3,

      /// <summary>
      /// TwoOpt algorithm enumerator.
      /// </summary>
      TwoOpt = 4,

      /// <summary>
      /// CitySwap algorithm enumerator.
      /// </summary>
      CitySwap = 5,

      /// <summary>
      /// LinKernighan algorithm enumerator.
      /// </summary>
      LinKernighan = 6,

      /// <summary>
      /// TabuSearch algorithm enumerator.
      /// </summary>
      TabuSearch = 7,

      /// <summary>
      /// VariableNeighborhoodSearch algorithm enumerator.
      /// </summary>
      VariableNeighborhoodSearch = 8
   }
}