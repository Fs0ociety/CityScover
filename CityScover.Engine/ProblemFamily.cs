//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 25/10/2018
//

namespace CityScover.Engine
{
   #region Enumerations
   public enum ProblemFamily
   {
      /// <summary>
      /// Invalid problem.
      /// </summary>
      None,

      /// <summary>
      /// Team orienteering problem.
      /// </summary>
      TeamOrienteering,

      /// <summary>
      /// Travelling Salesman problem. 
      /// </summary>
      TravellingSalesmanProblem,

      /// <summary>
      /// Vehicle routing problem.
      /// </summary>
      VehicleRoutingProblem,

      // ...
      // Add your own new problem's types here to expand new possible problems to solve.
   }
   #endregion
}