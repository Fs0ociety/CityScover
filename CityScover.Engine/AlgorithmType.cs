//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 16/10/2018
//

namespace CityScover.Engine
{
   #region Enumerations
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
      /// VariableNeighborhoodSearch algorithm enumerator.
      /// </summary>
      VariableNeighborhoodSearch = 7
   }
   #endregion
}