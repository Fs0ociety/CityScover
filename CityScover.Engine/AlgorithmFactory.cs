//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 10/09/2018
//

using CityScover.Engine.Algorithms.Greedy;
using System;

namespace CityScover.Engine
{
   /// <summary>
   /// Factory class for the Algorithm abstract interface.
   /// </summary>
   internal class AlgorithmFactory
   {
      #region Internal methods
      /// <summary>
      /// Creates the concrete instance of the Algorithm type.
      /// </summary>
      /// <param name="currentAlgorithm"></param>
      /// <returns></returns>
      internal static Algorithm CreateAlgorithm(AlgorithmType currentAlgorithm)
      {
         Algorithm algorithm = default;

         switch (currentAlgorithm)
         {
            case AlgorithmType.None:
               throw new ArgumentException("Invalid algorithm", nameof(currentAlgorithm));

            case AlgorithmType.NearestNeighbor:
               algorithm = new NearestNeighborAlgorithm();
               break;

            case AlgorithmType.NearestInsertion:
               break;

            case AlgorithmType.CheapestInsertion:
               break;

            case AlgorithmType.TwoOpt:
               break;

            case AlgorithmType.CitySwap:
               break;

            case AlgorithmType.LinKernighan:
               break;

            case AlgorithmType.IteratedLocalSearch:
               break;

            case AlgorithmType.TabuSearch:
               break;

            case AlgorithmType.VariableNeighborhoodSearch:
               break;

            default:
               break;
         }
         return algorithm;
      }
      #endregion
   }
}
