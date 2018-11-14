//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 10/11/2018
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

      public StageFlow(AlgorithmType algorithm, byte runningCount = 1)
      {
         CurrentAlgorithm = algorithm;
         RunningCount = runningCount;
         AlgorithmParameters = new Dictionary<ParameterCodes, dynamic>();
         ChildrenFlows = new Collection<StageFlow>();
      }
      #endregion

      #region Internal properties
      /// <summary>
      /// Current algorithm type for the current flow of a stage.
      /// </summary>
      public AlgorithmType CurrentAlgorithm { get; set; } = default;

      /// <summary>
      /// Maximum iterations for an algorithm.
      /// </summary>
      public byte RunningCount { get; set; } = default;
      public IDictionary<ParameterCodes, dynamic> AlgorithmParameters { get; set; }
      public ICollection<StageFlow> ChildrenFlows { get; set; }
      #endregion
   }
}