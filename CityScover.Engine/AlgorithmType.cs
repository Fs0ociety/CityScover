//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/11/2018
//

namespace CityScover.Engine
{
   #region AlgorithmType enumeration
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
      /// LinKernighan algorithm enumerator.
      /// </summary>
      LinKernighan = 5,

      /// <summary>
      /// TabuSearch algorithm enumerator.
      /// </summary>
      TabuSearch = 6,

      /// <summary>
      /// HybridNearestDistance algorithm enumerator.
      /// </summary>
      HybridNearestDistance = 7
   }
   #endregion
}