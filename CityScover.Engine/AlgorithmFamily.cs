﻿//
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
   #region AlgorithmFamily enumeration
   public enum AlgorithmFamily
   {
      /// <summary>
      /// Invalid algorithm category.
      /// </summary>
      None,

      /// <summary>
      /// Greedy algorithm's category.
      /// </summary>
      Greedy,

      /// <summary>
      /// Local search algorithm's category.
      /// </summary>
      LocalSearch,

      /// <summary>
      /// Improvements algorithm's category .
      /// (Hybrid between LocalSearch and (Meta)Heuristic)
      /// </summary>
      Improvement,

      /// <summary>
      /// (Meta)heuristic algorithm's category.
      /// </summary>
      MetaHeuristic,
   }
   #endregion
}