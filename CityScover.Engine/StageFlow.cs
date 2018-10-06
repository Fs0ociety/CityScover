//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 05/10/2018
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
   internal class StageFlow
   {
      #region Constructors
      internal StageFlow()
         : this(AlgorithmType.None, 1)
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
      internal AlgorithmType CurrentAlgorithm { get; set; }
      internal byte RunningCount { get; set; }
      internal ICollection<StageFlow> ChildrenFlows { get; set; }
      #endregion
   }
}
