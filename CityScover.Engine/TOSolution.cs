//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 27/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   internal class TOSolution
   {
      #region Private fields
      private static int _sequenceId;
      #endregion

      #region Constructors
      internal TOSolution()
      {
         Id = ++_sequenceId;
      }

      #endregion

      #region Internal properties
      /// <summary>
      /// Each Solution has an own ID.
      /// </summary>
      internal int Id { get; }

      /// <summary>
      /// Property used from SolverEvaluator to set a Cost for the Solution.
      /// </summary>
      internal int Cost { get; set; }      

      /// <summary>
      /// Property used from SolverEvaluator to set a Penalty for the Solution.
      /// </summary>
      internal int Penalty { get; set; }
      
      /// <summary>
      /// Property used from SolverValidator to check the TMax constraint.
      /// </summary>
      internal DateTime TimeSpent { get; set; }

      /// <summary>
      /// Property used from SolverValidator for analysis of problem's constraints.
      /// Contains the results of validation of constraints.
      /// </summary>
      internal IDictionary<byte, bool> ProblemConstraints { get; set; } = new Dictionary<byte, bool>();

      /// <summary>
      /// This is the internal structure formed by nodes and edges of Solution.
      /// </summary>
      internal CityMapGraph SolutionGraph { get; set; } = new CityMapGraph();

      internal bool IsValid => Penalty == 0;
      #endregion
   }
}
