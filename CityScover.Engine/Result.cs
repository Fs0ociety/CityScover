//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 06/10/2018
//

using System;
using System.Diagnostics;

namespace CityScover.Engine
{
   internal class Result
   {
      private TOSolution _currentSolution;
      private DateTime _timeSpent;
      private Validity _validity;
      private Stopwatch _runningTime;

      #region Constructors
      internal Result(TOSolution currentSolution, DateTime timeSpent, Validity validity = Validity.None)
      {
         _currentSolution = currentSolution;
         _timeSpent = timeSpent;
         _validity = validity;
      }
      #endregion

      #region Enumerations
      internal enum Validity
      {
         None,
         Valid,
         Invalid
      };
      #endregion

      #region Internal properties
      internal Stopwatch RunningTime
      {
         get => _runningTime;
         set => _runningTime = value;
      }
      #endregion

      #region Static methods
      internal static AlgorithmFamily GetAlgorithmFamilyByType(AlgorithmType algorithm)
      {
         AlgorithmFamily result = default; 

         switch (algorithm)
         {
            case AlgorithmType.NearestNeighbor:
            case AlgorithmType.NearestNeighborKnapsack:
            case AlgorithmType.CheapestInsertion:
               result = AlgorithmFamily.Greedy;
               break;

            case AlgorithmType.TwoOpt:
            case AlgorithmType.CitySwap:
               result = AlgorithmFamily.LocalSearch;
               break;

            case AlgorithmType.LinKernighan:
               result = AlgorithmFamily.Improvement;
               break;

            case AlgorithmType.TabuSearch:
            case AlgorithmType.VariableNeighborhoodSearch:
               result = AlgorithmFamily.MetaHeuristic;
               break;

            default:
               result = AlgorithmFamily.None;
               break;            
         }
         return result;
      }
      #endregion
   }
}