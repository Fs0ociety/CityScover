//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

using CityScover.Utils;
using System;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represent the Facade class of the CityScover.Engine, implemented as a Singleton.
   /// Contains the Execute method to run the configuration passed as argument.
   /// The Solver uses ExecutionTracer and SolverHelpers classes to do overall work.
   /// </summary>
   public sealed class Solver : Singleton<Solver>
   {
      #region Constructors
      private Solver()
      {
         // Il Solver crea il problema e lo trasmette all'ExecutionTracer.
         //Problem p = new Problem();
      }
      #endregion

      #region Internal methods (Factory methods)
      internal static ExecutionTracer CreateExecutionTracer()
      {
         throw new NotImplementedException();
      }

      internal static SolverConstraintHelper CreateSolverConstraintHelper()
      {
         throw new NotImplementedException();
      }

      internal static SolverEvaluationHelper CreateSolverEvaluationHelper()
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Public methods
      public void Execute(Configuration configuration)
      {
         if (configuration == null)
            throw new ArgumentNullException(nameof(Execute));

         throw new NotImplementedException(nameof(Execute));
      }
      #endregion
   }
}
