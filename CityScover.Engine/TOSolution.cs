//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 12/09/2018
//


using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   internal class TOSolution : BaseSolution
   {
      private double _cost = default;
      private double _penalty = default;

      #region Constructors
      internal TOSolution()
      { }
      #endregion

      #region Internal properties
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

      #region Overrides
      internal override double Cost
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

      internal override double Penalty
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
      #endregion
   }
}
