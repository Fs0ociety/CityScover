//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/09/2018
//

namespace CityScover.Entities
{
   public struct Route
   {
      #region Constructors
      public Route(int id, InterestPoint from, InterestPoint to, double distance) 
      {
         Id = id;
         PointFrom = from;
         PointTo = to;
         Distance = distance;
      }
      #endregion

      #region Public properties
      /// <summary>
      /// Identificativo univoco della tratta.
      /// </summary>
      public int Id { get; }
      
      /// <summary>
      /// Punto d'interesse di partenza.
      /// </summary>
      public InterestPoint PointFrom { get; private set; }
      
      /// <summary>
      /// Punto d'interesse di destinazione.
      /// </summary>
      public InterestPoint PointTo { get; private set; }

      /// <summary>
      /// Distanza della tratta. 
      /// </summary>
      public double Distance { get; }
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
