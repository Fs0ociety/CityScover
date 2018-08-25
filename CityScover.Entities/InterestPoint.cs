﻿//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using System;
using System.Collections.Generic;

namespace CityScover.Entities
{
   public class InterestPoint
   {
      #region Public properties
      /// <summary>
      /// Identificativo univoco del punto d'interesse.
      /// </summary>
      public int Id { get; set; }

      /// <summary>
      /// Descrizione generica del punto d'interesse.
      /// </summary>
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