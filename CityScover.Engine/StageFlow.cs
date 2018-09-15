//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 15/09/2018
//

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO: Tree structure for a stage execution flow.
   /// Built with a Composite Pattern (without children of different types, only the same type, like recursion).
   /// </summary>
   public class StageFlow
   {
      internal StageFlow()
         :this(AlgorithmType.None, 1)
      {
      }

      internal StageFlow(AlgorithmType algorithm, byte runningTimes)
      {
         CurrentAlgorithm = algorithm;
         RunningTimes = runningTimes;
         ChildrenFlows = new Collection<StageFlow>();
      }

      public AlgorithmType CurrentAlgorithm { get ; set; }
      public byte RunningTimes { get; set; }
      public ICollection<StageFlow> ChildrenFlows { get; set; }
   }
}
