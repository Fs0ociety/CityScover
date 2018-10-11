//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 11/10/2018
//

using CityScover.Engine.Algorithms;
using CityScover.Engine.Algorithms.Greedy;
using CityScover.Engine.Algorithms.Metaheuristics;
using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Algorithms.VariableDepthSearch;
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
      /// <param name="currentAlgorithm">
      /// Algorithm type to create.
      /// </param>
      /// <returns>
      /// The Algorithm's instance to run.
      /// </returns>
      internal static Algorithm CreateAlgorithm(AlgorithmType currentAlgorithm)
      {
         Algorithm algorithm = default;

         switch (currentAlgorithm)
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
               // TODO
               break;

            case AlgorithmType.TwoOpt:
               algorithm = new LocalSearch(new TwoOptNeighborhood());
               break;

            case AlgorithmType.CitySwap:
               // TODO
               break;

            case AlgorithmType.LinKernighan:
               algorithm = new LinKernighan();
               break;

            case AlgorithmType.TabuSearch:
               algorithm = new TabuSearch();
               break;

            case AlgorithmType.VariableNeighborhoodSearch:
               // TODO
               break;

            default:
               break;
         }
         return algorithm;
      }

      internal static Algorithm CreateAlgorithm(AlgorithmType currentAlgorithm, NeighborhoodType neighborhood)
      {
         Algorithm algorithm = default;
         Neighborhood neighborhoodWorker = default;

         switch (currentAlgorithm)
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
               // TODO
               break;

            case AlgorithmType.TwoOpt:
               neighborhoodWorker = NeighborhoodFactory.CreateNeighborhood(neighborhood);
               algorithm = new LocalSearch(neighborhoodWorker);
               break;

            case AlgorithmType.CitySwap:
               // TODO
               break;

            case AlgorithmType.LinKernighan:
               algorithm = new LinKernighan();
               break;

            case AlgorithmType.TabuSearch:
               neighborhoodWorker = NeighborhoodFactory.CreateNeighborhood(neighborhood);
               algorithm = new TabuSearch(neighborhoodWorker);
               break;

            case AlgorithmType.VariableNeighborhoodSearch:
               // TODO
               break;

            default:
               break;
         }
         return algorithm;
      }

      internal static Algorithm CreateAlgorithmByFamily(AlgorithmFamily family, 
         AlgorithmType algorithmType, NeighborhoodType neighborhoodType)
      {
         Algorithm algorithm = default;

         switch (family)
         {
            case AlgorithmFamily.None:
               break;

            case AlgorithmFamily.Greedy:
               algorithm = CreateAlgorithm(algorithmType, NeighborhoodType.None);
               break;

            case AlgorithmFamily.LocalSearch:
            case AlgorithmFamily.MetaHeuristic:
            case AlgorithmFamily.Improvement:
               algorithm = CreateAlgorithm(algorithmType, neighborhoodType);
               break;
         }
         return algorithm;
      }
      #endregion
   }
}
