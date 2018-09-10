//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 10/09/2018
//

namespace CityScover.Engine
{
   public enum ProblemType
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
}