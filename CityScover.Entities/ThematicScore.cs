//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/09/2018
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
      public TourCategory Category { get; set; }
      public int Value { get; set; }
      #endregion

      #region Public methods
      public ThematicScore DeepCopy() => (ThematicScore)MemberwiseClone();      
      #endregion
   }
}
