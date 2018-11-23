//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 10/11/2018
//

using System;
using System.Diagnostics;

namespace CityScover.Engine
{
   internal class Result
   {
      #region Private fields
      private TOSolution _currentSolution;
      private AlgorithmFamily _resultFamily;
      private AlgorithmType _runningAlgorithm;
      private DateTime? _timeSpent;
      private Validity _validity;
      private Stopwatch _runningTime;
      #endregion

      #region Constructors
      internal Result(TOSolution solution, AlgorithmType runningAlgorithm, 
         DateTime? time = null, Validity validity = Validity.None)
      {
         _currentSolution = solution;
         _runningAlgorithm = runningAlgorithm;
         _timeSpent = time;
         _validity = validity;
      }

      internal Result(Validity validity = Validity.None, params object[] paramsList)
      {
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
      internal AlgorithmFamily ResultFamily
      {
         get => _resultFamily;
         set => _resultFamily = value;
      }

      internal Stopwatch RunningTime
      {
         get => _runningTime;
         set => _runningTime = value;
      }

      public int Count => throw new NotImplementedException();

      public bool IsReadOnly => throw new NotImplementedException();
      #endregion

      #region Static methods
      internal static Result Create(params object[] paramsList)
      {
         throw new NotImplementedException(nameof(Create));
      }

      internal static AlgorithmFamily GetAlgorithmFamily(AlgorithmType algorithm)
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
               result = AlgorithmFamily.LocalSearch;
               break;

            case AlgorithmType.LinKernighan:
            case AlgorithmType.HybridDistanceInsertion:
            case AlgorithmType.HybridDistanceUpdate:
               result = AlgorithmFamily.Improvement;
               break;

            case AlgorithmType.TabuSearch:
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