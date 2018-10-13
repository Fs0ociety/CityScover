//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 13/10/2018
//

namespace CityScover.Engine
{
   /// <summary>
   /// ProblemFactory class to create the concrete ProblemBase derived classes.
   /// </summary>
   internal class ProblemFactory
   {
      #region Internal static methods
      /// <summary>
      /// Creates the concrete instance of the Problem type.
      /// </summary>
      /// <param name="currentProblem">
      /// Problem type to create.
      /// </param>
      /// <returns>
      /// The Problem's instance.
      /// </returns>
      internal static ProblemBase CreateProblem(ProblemFamily currentProblem)
      {
         ProblemBase problem = default;

         switch (currentProblem)
         {
            case ProblemFamily.None:
               break;

            case ProblemFamily.TravellingSalesmanProblem:
               break;

            case ProblemFamily.VehicleRoutingProblem:
               break;

            case ProblemFamily.TeamOrienteering:
               problem = new TOProblem();
               break;

            default:
               break;
         }
         return problem;
      }
      #endregion
   }
}