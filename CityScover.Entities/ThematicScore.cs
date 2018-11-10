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
   public struct ThematicScore
   {
      #region Constructors
      public ThematicScore(TourCategory category, int value)
      {
         Category = category;
         Value = value;
      }
      #endregion

      #region Public properties
      public TourCategory Category { get; }
      public int Value { get; }
      #endregion

      #region Public methods
      public ThematicScore DeepCopy() => (ThematicScore)MemberwiseClone();      
      #endregion
   }
}
