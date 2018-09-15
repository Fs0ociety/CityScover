//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 14/09/2018
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

      internal StageFlow(AlgorithmType type, byte runningTimes)
      {
         ChildrenFlows = new Collection<StageFlow>();
         Type = type;
         RunningTimes = runningTimes;
      }

      public ICollection<StageFlow> ChildrenFlows { get; set; }

      public AlgorithmType Type { get ; set; }
      public byte RunningTimes { get; set; }      
   }
}
