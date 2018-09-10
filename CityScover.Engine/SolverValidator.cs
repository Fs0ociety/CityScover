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
   internal sealed class SolverValidator : Singleton<SolverValidator>
   {
      #region Constructors
      private SolverValidator()
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
      private bool Validate(Solution solution, Problem problem)
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
