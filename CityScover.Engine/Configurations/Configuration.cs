//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/08/2018
//

using System.Collections.Generic;

namespace CityScover.Engine.Configurations
{
   /// <summary>
   /// TODO
   /// </summary>
   internal sealed class Configuration
   {
      private IDictionary<StageType, AlgorithmType> _stages;

      #region Constructors
      internal Configuration()
      {
         _stages = new Dictionary<StageType, AlgorithmType>();
      }
      #endregion

      #region Internal properties
      internal IDictionary<StageType, AlgorithmType> Stages => _stages;
      #endregion

      #region Internal methods
      internal void AddStage(StageType stage, AlgorithmType algorithm)
      {
         _stages.Add(stage, algorithm);
      } 
      #endregion
   }
}