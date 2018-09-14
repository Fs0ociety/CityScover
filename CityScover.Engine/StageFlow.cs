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
   public struct StageFlow
   {
      private ICollection<StageFlow> _childrenFlows;

      private AlgorithmType _type;

      private byte _runningTimes;

      internal StageFlow(byte runningTimes)
         :this(AlgorithmType.None, runningTimes)
      {
      }

      internal StageFlow(AlgorithmType type, byte runningTimes)
      {
         _childrenFlows = new Collection<StageFlow>();
         _type = type;
         _runningTimes = runningTimes;
      }      
   }
}
