//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/09/2018
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
      public TourCategoryType Id { get; set; }
      public string Description { get; set; }
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
