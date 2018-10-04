//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 04/10/2018
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

      public Result(TOSolution currentSolution, DateTime timeSpent, Validity validity = Validity.None)
      {
         _currentSolution = currentSolution;
         _timeSpent = timeSpent;
         _validity = validity;
      }

      internal enum Validity
      {
         None,
         Valid,
         Invalid
      };

      #region Internal properties
      internal Stopwatch RunningTime
      {
         get => _runningTime;
         set => _runningTime = value;
      }
      #endregion

      #region Static methods
      internal static ResultType GetResultTypeFromAlgorithmType(AlgorithmType algorithm)
      {
         ResultType result = default;

         switch (algorithm)
         {
            case AlgorithmType.NearestNeighbor:
            case AlgorithmType.NearestNeighborKnapsack:
            case AlgorithmType.CheapestInsertion:
               result = ResultType.Greedy;
               break;

            case AlgorithmType.TwoOpt:
            case AlgorithmType.CitySwap:
               result = ResultType.LocalSearch;
               break;

            case AlgorithmType.LinKernighan:
               result = ResultType.Heuristic;
               break;

            case AlgorithmType.TabuSearch:
            case AlgorithmType.VariableNeighborhoodSearch:
               result = ResultType.MetaHeuristic;
               break;

            default:
               result = ResultType.None;
               break;            
         }
         return result;
      }
      #endregion
   }
}