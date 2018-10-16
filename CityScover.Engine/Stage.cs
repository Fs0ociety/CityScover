//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 16/10/2018
//

namespace CityScover.Engine
{
   public class Stage
   {
      #region Constructors
      internal Stage()
      {
         Flow = new StageFlow();
      } 
      #endregion

      #region Public properties
      public StageType Description { get; set; }
      public AlgorithmFamily Category { get; set; }
      public StageFlow Flow { get; set; }
      #endregion
   }
}