//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/08/2018
//

using CityScover.Utils;
using System;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal sealed class SolverEvaluationHelper : Singleton<SolverEvaluationHelper>
   {
      private Configuration _workingConfiguration;

      #region Constructors
      private SolverEvaluationHelper()
      {
      }
      #endregion

      #region Internal properties
      internal Configuration WorkingConfiguration
      {
         get => _workingConfiguration;
         set => _workingConfiguration = value;
      }
      #endregion

      #region Internal methods
      internal double Evaluate(Solution solution, Problem problem)
      {
         throw new NotImplementedException();
      } 
      #endregion
   }
}
