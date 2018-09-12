//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 12/09/2018
//

namespace CityScover.Engine
{
   /// <summary>
   /// Represents a Solution provided from the execution of an Algorithm.
   /// The Solution maintain an internal graph structure formed by nodes and edges.
   /// </summary>
   internal abstract class BaseSolution
   {
      #region Constructors
      internal BaseSolution()
      { }
      #endregion

      #region Internal properties
      /// <summary>
      /// Each Solution has an own ID.
      /// </summary>
      internal int? Id { get; set; }

      /// <summary>
      /// Property used from SolverEvaluator to set a Cost for the Solution.
      /// </summary>
      internal abstract double Cost { get; set; }

      /// <summary>
      /// Property used from SolverEvaluator to set a Penalty for the Solution.
      /// </summary>
      internal abstract double Penalty { get; set; }
      #endregion
   }
}
