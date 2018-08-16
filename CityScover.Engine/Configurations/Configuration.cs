//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine.Configurations
{
   /// <summary>
   /// TODO
   /// </summary>
   internal sealed class Configuration
   {
      //private IDictionary<StageType, AlgorithmType> _stages;
      private ICollection<StageType> _stages;

      #region Constructors
      internal Configuration()
      {
         //_stages = new Dictionary<StageType, AlgorithmType>();
         _stages = new Collection<StageType>();
      }
      #endregion

      #region Internal properties
      //internal IDictionary<StageType, AlgorithmType> Stages => _stages;
      internal IEnumerable<StageType> Stages;
      #endregion

      #region Internal methods
      internal void AddStage(StageType stage)
      {
         //_stages.Add(stage, algorithm);
         _stages.Add(stage);
      } 
      #endregion
   }
}