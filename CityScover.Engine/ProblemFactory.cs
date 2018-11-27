//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/11/2018
//

namespace CityScover.Engine
{
   /// <summary>
   /// ProblemFactory class to create the concrete ProblemBase derived classes.
   /// </summary>
   internal static class ProblemFactory
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
               // TODO... Creation of "Travelling Salesman problem"
               break;

            case ProblemFamily.VehicleRoutingProblem:
               // TODO... Creation of "Vehicle Routing problem"
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