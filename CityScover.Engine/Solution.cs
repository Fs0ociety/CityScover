//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 07/09/2018
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
      {
         ProblemConstraints = new Dictionary<byte, bool>();
         SolutionGraph = new CityMapGraph();
         Evaluation = new Evaluation();
      }
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
      internal IDictionary<byte, bool> ProblemConstraints { get; set; }

      /// <summary>
      /// This is the internal structure formed by nodes and edges of Solution.
      /// </summary>
      internal CityMapGraph SolutionGraph { get; set; }

      /// <summary>
      /// Property used from SolverEvaluator to set a Cost for the Solution.
      /// The Cost can be assigned a Penalty.
      /// </summary>
      internal Evaluation Evaluation { get; set; }
      #endregion
   }
}
