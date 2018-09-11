//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 11/09/2018
//

using CityScover.Engine.Workers;
using System.Collections.Generic;

namespace CityScover.Engine
{
   /// <summary>
   /// Represents a Solution provided from the execution of an Algorithm.
   /// The Solution maintain an internal graph structure formed by nodes and edges.
   /// </summary>
   internal class Solution
   {
      #region Constructors
      internal Solution()
      { }
      #endregion

      #region Internal properties
      /// <summary>
      /// Each Solution has an own ID.
      /// </summary>
      internal int? Id { get; set; }

      /// <summary>
      /// Property used from SolverValidator for analysis of problem's constraints.
      /// Contains the results of validation of constraints.
      /// </summary>
      internal IDictionary<byte, bool> ProblemConstraints { get; set; } = new Dictionary<byte, bool>();

      /// <summary>
      /// This is the internal structure formed by nodes and edges of Solution.
      /// </summary>
      internal CityMapGraph SolutionGraph { get; set; } = new CityMapGraph();

      /// <summary>
      /// Property used from SolverEvaluator to set a Cost for the Solution.
      /// </summary>
      internal double Cost { get; set; }

      /// <summary>
      /// Property used from SolverEvaluator to set a Penalty for the Solution.
      /// </summary>
      internal double Penalty { get; set; }
      #endregion
   }
}
