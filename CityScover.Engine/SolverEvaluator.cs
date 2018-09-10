//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 08/09/2018
//

using CityScover.Utils;
using System;
using System.Threading.Tasks;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal sealed class SolverEvaluator : Singleton<SolverEvaluator>
   {
      #region Constructors
      private SolverEvaluator()
      {
      }
      #endregion

      #region Internal properties
      internal Configuration WorkingConfiguration
      {
         get => Solver.Instance.WorkingConfiguration;
      }
      #endregion

      #region Private methods
      private double Evaluate(Solution solution, IProblem problem)
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Internal methods
      internal async Task Run()
      {
         throw new NotImplementedException();
      }
      #endregion
   }
}
