//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 23/10/2018
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
         : this(AlgorithmType.None, 1)
      {
      }

      internal StageFlow(AlgorithmType algorithm, byte runningCount, bool canExecuteImprovements = true)
      {
         if (runningCount == 0)
         {
            throw new ArgumentException($"Bad configuration format: " +
               $"{nameof(runningCount)} parameter must be greater than 0.");
         }

         CurrentAlgorithm = algorithm;
         RunningCount = runningCount;
         CanExecuteImprovements = canExecuteImprovements;
         ChildrenFlows = new Collection<StageFlow>();
      }
      #endregion

      #region Internal properties
      public AlgorithmType CurrentAlgorithm { get; set; }
      public byte RunningCount { get; set; }
      public bool CanExecuteImprovements { get; set; }
      public byte MaximumDeadlockIterations { get; set; }
      public ushort ImprovementThreshold { get; set; }
      public byte MaxIterationsWithoutImprovements { get; set; }
      public ICollection<StageFlow> ChildrenFlows { get; set; }
      #endregion
   }
}

