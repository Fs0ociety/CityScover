using System;
using System.Collections.Generic;

namespace CityScover.Entities
{
   public class InterestPoint
   {
      #region Public properties
      public string Name { get; set; }

      /// <summary>
      /// Categoria di appartenenza del punto d'interesse.
      /// </summary>
      public TourCategory Category { get; set; }
      
      /// <summary>
      /// Punteggio tematico assegnato al punto d'interesse.
      /// </summary>
      public ThematicScore Score { get; set; }
      
      /// <summary>
      /// Orari di apertura del punto d'interesse.
      /// </summary>
      public ICollection<IntervalTime> OpeningTimes { get; set; }
      
      /// <summary>
      /// Tempo necessario per visitare il punto d'interesse.
      /// </summary>
      public TimeSpan? TimeVisit { get; set; }
      #endregion
   }
}