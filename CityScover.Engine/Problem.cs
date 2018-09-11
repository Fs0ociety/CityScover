//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 11/09/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal abstract class Problem
   {
      internal struct Constraint
      {
         /// <summary>
         /// Abstraction of problem's constraint.
         /// </summary>
         /// <param name="id"></param>
         /// <param name="logic"></param>
         internal Constraint(byte id, Func<Solution, bool> logic)
         {
            Id = id;
            Logic = logic;
         }

         internal byte Id { get; set; }
         internal Func<Solution, bool> Logic { get; set; }
      }

      #region Constructors
      internal Problem()
      {
      }
      #endregion

      #region Internal properties
      internal Func<Solution, Evaluation> ObjectiveFunc { get; set; } = default;
      internal ICollection<Constraint> Constraints { get; } = new Collection<Constraint>();

      /*
       * ALTERNATIVA
       * 
       * Al posto di definire una struct Constraint definita da noi è possibile usare la struct predefinita di .NET 
       * KeyValuePair<TKey, TValue>, dove:
       * 
       * TKey: rappresenta l'ID del Constraint
       * TValue: rappresenta il delegato che implementa la logica di validazione del vincolo con ID corrispondente.
       * 
       * In tal modo si crea una corrispondenza tra ID del vincolo e sua logica di validazione.
       * 
       * Ad esempio:
       * La Collection Constraints2 che segue è una Collection formata da un insieme di coppie ID-Delegate come segue:
       * 
       * Constraints2:
       *    KeyValuePair<1, IsTMaxConstraintSatisfied>
       *    KeyValuePair<2, IsTimeWindowsConstraintSatisfied>
       *    KeyValuePair<3, ...>
       *    KeyValuePair<..., ...>
       *    KeyValuePair<n, Is...>
       *    ...
       */
      internal ICollection<KeyValuePair<byte, Func<Solution, bool>>> Constraints2 { get; } = new Collection<KeyValuePair<byte, Func<Solution, bool>>>();
      #endregion
   }
}
