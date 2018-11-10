//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/11/2018
//

namespace CityScover.Entities
{
   public struct MeasureUnit
   {
      #region Constructors
      public MeasureUnit(string code, string symbol)
      {
         Code = code;
         Symbol = symbol;
      } 
      #endregion

      #region Public properties
      public string Code { get; }

      public string Symbol { get; }
      #endregion
   }
}
