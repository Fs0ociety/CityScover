﻿//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 30/07/2018
//

using System;
using CityScover.ADT.Graphs;

namespace CityScover.Entities
{
   public class Route : IGraphEdgeWeight
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
      public float Distance { get; set; }

      public Func<double> Weight => () => Distance;
      #endregion
    }
}