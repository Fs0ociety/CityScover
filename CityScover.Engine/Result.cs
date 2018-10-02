//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 01/10/2018
//

using System;
using CityScover.Engine.Workers;

namespace CityScover.Engine
{
   internal class Result
   {
      private TOSolution _currentSolution;
      private DateTime _timeSpent;
      private ResultType _resultType;
      private Validity _validity;

      public Result(TOSolution currentSolution, DateTime timeSpent, ResultType resultType = ResultType.None, Validity validity = Validity.None)
      {
         _currentSolution = currentSolution;
         _timeSpent = timeSpent;
         _resultType = resultType;
         _validity = validity;
      }

      internal enum Validity
      {
         None,
         Valid,
         Invalid
      };
   }
}