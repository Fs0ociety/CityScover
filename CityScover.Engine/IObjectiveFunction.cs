//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 22/08/2018
//

namespace CityScover.Engine
{
   /// <summary>
   /// It represents the Operative Research Objective Function.
   /// </summary>
   internal interface IObjectiveFunction
   {
      /// <summary>
      /// Basically it computes the solution cost.
      /// </summary>
      /// <param name="solution"></param>
      /// <returns>
      /// An Evaluation Object.
      /// </returns>
      Evaluation CalculateCost(Solution solution);
   }
}
