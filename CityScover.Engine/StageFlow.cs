﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 10/12/2018
//

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
         : this(AlgorithmType.None)
      {
      }

      public StageFlow(AlgorithmType algorithm)
      {
         CurrentAlgorithm = algorithm;
         AlgorithmParameters = new Dictionary<ParameterCodes, dynamic>();
         ChildrenFlows = new Collection<StageFlow>();
      }
      #endregion

      #region Internal properties
      /// <summary>
      /// Current algorithm type for the current flow of a stage.
      /// </summary>
      public AlgorithmType CurrentAlgorithm { get; set; }

      /// <summary>
      /// Specific parameters of the algorithm.
      /// </summary>
      public IDictionary<ParameterCodes, dynamic> AlgorithmParameters { get; }
      public ICollection<StageFlow> ChildrenFlows { get; }
      #endregion
   }
}