//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 25/10/2018
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
      public StageFlow()
         : this(AlgorithmType.None, runningCount: 1)
      {
      }

      public StageFlow(AlgorithmType algorithm, byte runningCount)
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

      /// <summary> Current algorithm type for the current flow of a stage. </summary>
      public AlgorithmType CurrentAlgorithm { get; set; } = default;

      /// <summary> Maximum iterations for an algorithm. </summary>
      public byte RunningCount { get; set; } = default;

      /// <summary> Flag that states if an algorithm can execute an improvement logic. </summary>
      public bool CanExecuteImprovements { get; set; } = true;

      /// <summary> Lin Kernighan threshold to trigger improvement logic. </summary>
      public ushort LkImprovementThreshold { get; set; } = default;

      /// <summary>
      /// Stopping condition for Local Search algorithm. 
      /// It verifies if there are one or more iterations without solution cost improvements 
      /// during algorithm's execution.
      /// </summary>
      public byte MaxIterationsWithoutImprovements { get; set; } = default;

      /// <summary>
      /// Stopping condition for MetaHeuristic algorithm. 
      /// It verifies if there is a deadlock in the algorithm's execution.
      /// </summary>
      public byte MaximumDeadlockIterations { get; set; } = default;

      /// <summary> Hybrid Nearest Distance algorithm's threshold for Tmax constraint. </summary>
      public TimeSpan HndTmaxThreshold { get; set; } = default;

      public ICollection<StageFlow> ChildrenFlows { get; set; }
      #endregion
   }
}