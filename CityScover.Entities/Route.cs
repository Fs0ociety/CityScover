//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/09/2018
//

namespace CityScover.Entities
{
   public class Route
   {
      #region Public properties
      /// <summary>
      /// Identificativo univoco della tratta.
      /// </summary>
      public int Id { get; set; }
      
      /// <summary>
      /// Punto d'interesse di partenza.
      /// </summary>
      public InterestPoint PointFrom { get; set; }
      
      /// <summary>
      /// Punto d'interesse di destinazione.
      /// </summary>
      public InterestPoint PointTo { get; set; }

      /// <summary>
      /// Distanza della tratta. 
      /// </summary>
      public double Distance { get; set; }
      #endregion

      #region Public methods
      public Route DeepCopy()
      {
         Route copy = (Route)MemberwiseClone();
         copy.PointFrom = PointFrom.DeepCopy();
         copy.PointTo = PointTo.DeepCopy();
         return copy;
      } 
      #endregion
   }
}
