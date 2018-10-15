//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 13/10/2018
//

using CityScover.Engine.Algorithms;
using CityScover.Engine.Algorithms.Greedy;
using CityScover.Engine.Algorithms.Metaheuristics;
using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Algorithms.VariableDepthSearch;
using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   /// <summary>
   /// Factory class for the Algorithm abstract interface.
   /// </summary>
   internal class AlgorithmFactory
   {
      #region Internal static methods
      /// <summary>
      /// Creates the concrete instance of the Algorithm type.
      /// </summary>
      /// <param name="algorithmType"> Algorithm type to create. </param>
      /// <returns> The Algorithm's instance to run. </returns>
      internal static Algorithm CreateAlgorithm(AlgorithmType algorithmType)
      {
         Algorithm algorithm = default;
         Neighborhood neighborhood = default;

         switch (algorithmType)
         {
            case AlgorithmType.None:
               break;

            case AlgorithmType.NearestNeighbor:
               algorithm = new NearestNeighbor();
               break;

            case AlgorithmType.NearestNeighborKnapsack:
               algorithm = new NearestNeighborKnapsack();
               break;

            case AlgorithmType.CheapestInsertion:
               // ... TODO ...
               break;

            case AlgorithmType.TwoOpt:
               neighborhood = NeighborhoodFactory.CreateNeighborhood(algorithmType);
               algorithm = new LocalSearch(neighborhood);
               break;

            case AlgorithmType.LinKernighan:
               algorithm = new LinKernighan();
               break;

            case AlgorithmType.TabuSearch:
               neighborhood = NeighborhoodFactory.CreateNeighborhood(algorithmType);
               algorithm = new TabuSearch(neighborhood);
               break;

            // Add new Algorithm types here ...

            default:
               break;
         }
         return algorithm;
      }

      /// <summary>
      /// Create the concrete instance of the Algorithm with a specified Neighborhood as parameter.
      /// </summary>
      /// <param name="currentAlgorithm"> Algorithm type to create. </param>
      /// <param name="neighborhood"> Neighborhood type to pass to the Algorithm. </param>
      /// <returns> The Algorithm's instance to run. </returns>
      internal static Algorithm CreateAlgorithm(AlgorithmType currentAlgorithm, Neighborhood neighborhood)
      {
         if (neighborhood == null)
         {
            throw new ArgumentNullException(nameof(neighborhood));
         }

         Algorithm algorithm = default;

         switch (currentAlgorithm)
         {
            case AlgorithmType.None:
               break;

            case AlgorithmType.TwoOpt:
               algorithm = new LocalSearch(neighborhood);
               break;

            default:
               break;
         }
         return algorithm;
      }

      internal static Algorithm CreateAlgorithm<TNeighborhood>(AlgorithmType currentAlgorithm, TNeighborhood neighborhood)
         where TNeighborhood : Neighborhood
      {
         if (EqualityComparer<TNeighborhood>.Default.Equals(neighborhood, default))
         {
            throw new ArgumentNullException(nameof(neighborhood));
         }

         Algorithm algorithm = default;

         switch (currentAlgorithm)
         {
            case AlgorithmType.None:
               break;

            case AlgorithmType.TwoOpt:
               algorithm = new LocalSearch(neighborhood);
               break;

            default:
               break;
         }
         return algorithm;
      }
      #endregion
   }
}