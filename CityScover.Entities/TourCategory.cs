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
   public struct TourCategory
   {
      #region Constructors
      public TourCategory(TourCategoryType id, string description)
      {
         Id = id;
         Description = description;
      }
      #endregion

      #region Public properties
      public TourCategoryType Id { get; }
      public string Description { get; private set; }
      #endregion

      #region Public methods
      public TourCategory DeepCopy()
      {
         TourCategory copy = (TourCategory)MemberwiseClone();
         copy.Description = string.Copy(Description);
         return copy;
      } 
      #endregion
   }
}
