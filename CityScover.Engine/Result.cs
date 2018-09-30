﻿//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 30/09/2018
//

using System;
using CityScover.Engine.Workers;

namespace CityScover.Engine
{
   internal class Result
   {
      private CityMapGraph _currentSolution;
      private ushort _currentStep;
      private DateTime _timeSpent;
      private ResultType _resultType;

      public Result(CityMapGraph currentSolution, ushort currentStep, DateTime timeSpent, ResultType resultType = ResultType.None)
      {
         _currentSolution = currentSolution;
         _currentStep = currentStep;
         _timeSpent = timeSpent;
         _resultType = resultType;
      }
   }
}