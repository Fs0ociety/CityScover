﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/12/2018
//

using CityScover.Engine.Algorithms.CustomAlgorithms;
using CityScover.Engine.Algorithms.Greedy;
using CityScover.Engine.Algorithms.LocalSearches;
using CityScover.Engine.Algorithms.Metaheuristics;
using CityScover.Engine.Algorithms.Neighborhoods;
using CityScover.Engine.Algorithms.VariableDepthSearch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   /// <summary>
   /// Factory class for the Algorithm abstract interface.
   /// </summary>
   internal static class AlgorithmFactory
   {
      #region Internal static methods
      /// <summary>
      /// Creates the concrete instance of the Algorithm type.
      /// </summary>
      /// <param name="algorithmType"> Algorithm type to create. </param>
      /// <returns> The Algorithm's instance to run. </returns>
      internal static Algorithm CreateAlgorithm(AlgorithmType algorithmType)
      {
         return CreateAlgorithm(algorithmType, null);
      }

      /// <summary>
      /// Create the concrete instance of the Algorithm with a specified Neighborhood as parameter.
      /// </summary>
      /// <param name="currentAlgorithm"> Algorithm type to create. </param>
      /// <param name="neighborhood"> Neighborhood type to pass to the Algorithm. </param>
      /// <returns> The Algorithm's instance to run. </returns>
      internal static Algorithm CreateAlgorithm(AlgorithmType algorithmType, Neighborhood<ToSolution> neighborhood)
      {
         Algorithm algorithm = default;

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
               algorithm = new CheapestInsertion();
               break;

            case AlgorithmType.TwoOpt:
               if (neighborhood == null)
               {
                  neighborhood = NeighborhoodFactory.CreateNeighborhood(algorithmType);
               }               
               algorithm = new LocalSearchTemplate(neighborhood);
               break;
            
            case AlgorithmType.LinKernighan:
               algorithm = new LinKernighan();
               break;

            case AlgorithmType.HybridCustomInsertion:
               algorithm = new HybridCustomInsertion();
               break;

            case AlgorithmType.HybridCustomUpdate:
               algorithm = new HybridCustomUpdate();
               break;

            case AlgorithmType.TabuSearch:
               if (neighborhood == null)
               {
                  neighborhood = NeighborhoodFactory.CreateNeighborhood(algorithmType);
               }
               algorithm = new TabuSearch(neighborhood);
               break;

            // Add new Algorithm types here ...
            default:
               throw new ArgumentOutOfRangeException(nameof(algorithmType), algorithmType, null);
         }
         return algorithm;
      }

      internal static IEnumerable<Algorithm> GetImprovementAlgorithms(IEnumerable<StageFlow> childrenFlows)
      {
         if (!childrenFlows.Any())
         {
            yield break;
         }

         foreach (var child in childrenFlows)
         {
            var algorithm = CreateAlgorithm(child.CurrentAlgorithm);

            if (algorithm is null)
            {
               throw new InvalidOperationException("Bad configuration format: " +
                  $"{nameof(Solver.WorkingConfiguration)}.");
            }

            algorithm.Parameters = child.AlgorithmParameters;
            yield return algorithm;
         }
      }

      #region Generic version
      //internal static Algorithm CreateAlgorithm<TNeighborhood>(AlgorithmType currentAlgorithm, TNeighborhood neighborhood)
      //   where TNeighborhood : Neighborhood
      //{
      //   if (EqualityComparer<TNeighborhood>.Default.Equals(neighborhood, default))
      //   {
      //      throw new ArgumentNullException(nameof(neighborhood));
      //   }

      //   Algorithm algorithm = default;

      //   switch (currentAlgorithm)
      //   {
      //      case AlgorithmType.None:
      //         break;

      //      case AlgorithmType.TwoOpt:
      //         algorithm = new LocalSearchTemplate(neighborhood);
      //         break;
      //      case AlgorithmType.NearestNeighbor:
      //         break;
      //      case AlgorithmType.NearestNeighborKnapsack:
      //         break;
      //      case AlgorithmType.CheapestInsertion:
      //         break;
      //      case AlgorithmType.LinKernighan:
      //         break;
      //      case AlgorithmType.TabuSearch:
      //         break;
      //      case AlgorithmType.HybridCustomInsertion:
      //         break;
      //      case AlgorithmType.HybridCustomUpdate:
      //         break;
      //      default:
      //         throw new ArgumentOutOfRangeException(nameof(currentAlgorithm), currentAlgorithm, null);
      //   }
      //   return algorithm;
      //}
      #endregion

      #endregion
   }
}