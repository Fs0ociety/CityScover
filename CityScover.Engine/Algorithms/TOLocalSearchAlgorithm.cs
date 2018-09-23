//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 23/09/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms
{
   internal class TOLocalSearchAlgorithm : Algorithm
   {
      private double _previousSolutionCost;
      private double _currentSolutionCost;
      private CityMapGraph _currentSolution;

      private TONeighborhood _neighborhood;

      #region Constructors
      internal TOLocalSearchAlgorithm(TONeighborhood neighborhood)
         : this(neighborhood, null)
      {
      }

      public TOLocalSearchAlgorithm(TONeighborhood neighborhood, AlgorithmTracker provider)
         : base(provider)
      {
      }
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();

      }

      internal override void PerformStep()
      {
         throw new System.NotImplementedException();
      }

      internal override bool StopConditions()
      {
         return _previousSolutionCost != _currentSolutionCost ||
            _status == AlgorithmStatus.Terminating;
      } 
      #endregion
   }
}
