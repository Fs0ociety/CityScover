//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 13/09/2018
//

namespace CityScover.Engine
{
   /// <summary>
   /// Create the concrete instance of the specific problem.
   /// </summary>
   internal class ProblemFactory
   {
      #region Internal methods
      /// <summary>
      /// Creates the concrete instance of the Problem type.
      /// </summary>
      /// <param name="currentProblem">
      /// Problem type to create.
      /// </param>
      /// <returns>
      /// The Problem's instance.
      /// </returns>
      internal static Problem CreateProblem(ProblemFamily currentProblem)
      {
         Problem problem = default;

         switch (currentProblem)
         {
            case ProblemFamily.None:
               break;

            case ProblemFamily.TeamOrienteering:
               problem = new TOProblem();
               break;

            case ProblemFamily.TravellingSalesmanProblem:
               break;

            case ProblemFamily.VehicleRoutingProblem:
               break;

            default:
               break;
         }
         return problem;
      }
      #endregion
   }
}