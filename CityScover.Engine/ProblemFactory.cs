//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 10/09/2018
//

using System;

namespace CityScover.Engine
{
   /// <summary>
   /// Create the concrete instance of the specific problem.
   /// </summary>
   internal class ProblemFactory
   {
      #region Internal methods
      internal static Problem CreateProblem(ProblemType currentProblem)
      {
         Problem problem = default;

         switch (currentProblem)
         {
            case ProblemType.None:
               throw new ArgumentException("Invalid problem", nameof(currentProblem));

            case ProblemType.TeamOrienteering:
               problem = new TOProblem();
               break;

            case ProblemType.TravellingSalesmanProblem:
               break;

            case ProblemType.VehicleRoutingProblem:
               break;

            default:
               break;
         }
         return problem;
      }
      #endregion
   }
}