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
      /// NearestNeighbor algorithm enumerator.
      /// </summary>
      NearestNeighbor = 1,

      /// <summary>
      /// NearestInsertion algorithm enumerator.
      /// </summary>
      NearestInsertion = 2,

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
      /// IteratedLocalSearch algorithm enumerator.
      /// </summary>
      IteratedLocalSearch = 7,

      /// <summary>
      /// TabuSearch algorithm enumerator.
      /// </summary>
      TabuSearch = 8,

      /// <summary>
      /// VariableNeighborhoodSearch algorithm enumerator.
      /// </summary>
      VariableNeighborhoodSearch = 9
   }
}