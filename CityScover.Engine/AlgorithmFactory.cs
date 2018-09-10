//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 10/09/2018
//

using CityScover.Engine.Algorithms.Greedy;

namespace CityScover.Engine
{
   /// <summary>
   /// Factory class for the Algorithm abstract intercface.
   /// </summary>
   internal class AlgorithmFactory
   {
      #region Private methods
      private static Algorithm Create<TAlgorithm>()
         where TAlgorithm : Algorithm, new()
      {
         return new TAlgorithm();
      }
      #endregion

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
            case AlgorithmType.NearestNeighbor:
               algorithm = Create<NearestNeighborAlgorithm>();
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

      //internal static (Algorithm algorithm, AlgorithmTracker provider) CreateAlgorithm(AlgorithmType currentAlgorithm)
      //{
      //   Algorithm algorithm = default;
      //   AlgorithmTracker provider = new AlgorithmTracker();

      //   switch (currentAlgorithm)
      //   {
      //      case AlgorithmType.NearestNeighbor:
      //         algorithm = Create<NearestNeighborAlgorithm>();
      //         algorithm.Provider = provider;
      //         break;

      //      case AlgorithmType.NearestInsertion:
      //         break;

      //      case AlgorithmType.CheapestInsertion:
      //         break;

      //      case AlgorithmType.TwoOpt:
      //         break;

      //      case AlgorithmType.CitySwap:
      //         break;

      //      case AlgorithmType.LinKernighan:
      //         break;

      //      case AlgorithmType.IteratedLocalSearch:
      //         break;

      //      case AlgorithmType.TabuSearch:
      //         break;

      //      case AlgorithmType.VariableNeighborhoodSearch:
      //         break;

      //      default:
      //         provider = null;
      //         break;
      //   }

      //   return (algorithm, provider);
      //}
      #endregion
   }
}
