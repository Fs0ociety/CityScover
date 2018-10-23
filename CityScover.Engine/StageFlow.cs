//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 22/10/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// Tree structure for a stage execution flow.
   /// Built with a Composite Pattern (without children of different types, only the same type, like recursion).
   /// </summary>
   public class StageFlow
   {
      #region Constructors
      internal StageFlow()
         : this(AlgorithmType.None, runningCount: 1)
      {
      }

      internal StageFlow(AlgorithmType algorithm, byte runningCount)
      {
         if (runningCount == 0)
         {
            throw new ArgumentException($"Bad configuration format: " +
               $"{nameof(runningCount)} parameter must be greater than 0.");
         }

         CurrentAlgorithm = algorithm;
         RunningCount = runningCount;
         ChildrenFlows = new Collection<StageFlow>();
      }
      #endregion

      #region Internal properties
      public AlgorithmType CurrentAlgorithm { get; set; } = default;
      public byte RunningCount { get; set; } = default;
      public bool CanExecuteImprovements { get; set; } = true;
      public byte ImprovementThreshold { get; set; } = default;
      public byte MaxIterationsWithImprovements { get; set; } = default;
      public byte MaximumDeadlockIterations { get; set; } = default;
      public ICollection<StageFlow> ChildrenFlows { get; set; } = default;
      #endregion
   }
}

