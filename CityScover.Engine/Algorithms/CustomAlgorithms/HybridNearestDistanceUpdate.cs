//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 29/10/2018
//

using System;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.CustomAlgorithms
{
   internal class HybridNearestDistanceUpdate : HybridNearestDistanceInsertion
   {
      #region Constructors
      internal HybridNearestDistanceUpdate()
         :this(null)
      {
      }

      internal HybridNearestDistanceUpdate(AlgorithmTracker provider) 
         : base(provider)
      {
      }
      #endregion

      #region Private methods
      #endregion

      #region Overrides
      internal override void OnInitializing()
      {
         base.OnInitializing();
      }

      internal override async Task PerformStep()
      {
         // TODO
      }

      internal override void OnError(Exception exception)
      {
         base.OnError(exception);
      }

      internal override void OnTerminating()
      {
         base.OnTerminating();
      }

      internal override void OnTerminated()
      {
         base.OnTerminated();
      }

      internal override bool StopConditions()
      {
         return base.StopConditions();
      }
      #endregion
   }
}
