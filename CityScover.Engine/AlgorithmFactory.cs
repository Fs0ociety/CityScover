//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 05/10/2018
//

using CityScover.Engine.Algorithms;
using CityScover.Engine.Algorithms.Greedy;
using CityScover.Engine.Algorithms.Metaheuristics;
using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Algorithms.VariableDepthSearch;
using CityScover.Engine.Workers;
using System.Collections.Generic;

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
               break;

            case AlgorithmType.TwoOpt:
               algorithm = new LocalSearch<RouteWorker, IEnumerable<RouteWorker>>(new TwoOptNeighborhood());
               break;

            case AlgorithmType.CitySwap:
               break;

            case AlgorithmType.LinKernighan:
               algorithm = new LinKernighan();
               break;

            case AlgorithmType.TabuSearch:
               algorithm = new TabuSearch();
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
