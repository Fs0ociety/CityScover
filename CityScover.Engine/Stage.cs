//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// 
// File update: 12/10/2018
//

namespace CityScover.Engine
{
   internal class Stage
   {
      #region Constructors
      internal Stage()
      {
         Flow = new StageFlow();
      } 
      #endregion

      #region Public properties
      internal StageType Description { get; set; }
      internal AlgorithmFamily Category { get; set; }
      internal StageFlow Flow { get; set; }
      #endregion
   }
}