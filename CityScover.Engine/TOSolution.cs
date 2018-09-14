//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 14/09/2018
//


using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   internal class TOSolution
   {
      private double _cost = default;
      private double _penalty = default;

      #region Constructors
      internal TOSolution()
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
      internal double Cost
      {
         get => _cost;
         set
         {
            if (value != _cost)
            {
               _cost = value;
            }
         }
      }

      /// <summary>
      /// Property used from SolverEvaluator to set a Penalty for the Solution.
      /// </summary>
      internal double Penalty
      {
         get => _penalty;
         set
         {
            if (value != _penalty)
            {
               _penalty = value;
            }
         }
      }

      /// <summary>
      /// Property used from SolverValidator for analysis of problem's constraints.
      /// Contains the results of validation of constraints.
      /// </summary>
      internal IDictionary<byte, bool> ProblemConstraints { get; set; } = new Dictionary<byte, bool>();

      /// <summary>
      /// This is the internal structure formed by nodes and edges of Solution.
      /// </summary>
      internal CityMapGraph SolutionGraph { get; set; } = new CityMapGraph();
      #endregion      
   }
}
