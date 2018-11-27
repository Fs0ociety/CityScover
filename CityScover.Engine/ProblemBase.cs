//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 07/11/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// This abstract class represents a template for a generic problem.
   /// </summary>
   internal abstract class ProblemBase
   {
      #region Constructors
      internal ProblemBase()
      {
         Constraints = new Collection<KeyValuePair<string, Func<ToSolution, bool>>>();
      }
      #endregion

      #region Abstract members
      /// <summary>
      /// Abstract automatic properties doesn't create a private backing field.
      /// </summary>
      internal abstract Func<ToSolution, int> ObjectiveFunc { get; set; }

      internal abstract Func<ToSolution, int> PenaltyFunc { get; set; }
      #endregion

      #region Internal properties
      internal ICollection<KeyValuePair<string, Func<ToSolution, bool>>> Constraints { get; }
      internal bool IsMinimizing { get; set; } = true;
      #endregion

      #region Internal methods

      /// <summary>
      /// Ritorna il miglior costo tra cost1 e cost2, a seconda che il
      /// problema sia di massimo o di minimo. Ad esempio, se il problema è di
      /// massimo, la funzione ritornerà true se cost1 > cost2.
      /// </summary>
      /// <param name="firstSolutionCost">Costo prima soluzione</param>
      /// <param name="secondSolutionCost">Costo seconda soluzione</param>
      /// <param name="considerEqualityComparison">Valutare il confronto con uguaglianza</param>
      /// <returns>Il confronto da fare.</returns>
      internal bool CompareSolutionsCost(int firstSolutionCost, int secondSolutionCost, bool considerEqualityComparison = false)
      {
         return IsMinimizing ? 
            (considerEqualityComparison ? 
            firstSolutionCost <= secondSolutionCost : 
            firstSolutionCost < secondSolutionCost)
            
            : (considerEqualityComparison ? 
            firstSolutionCost >= secondSolutionCost : 
            firstSolutionCost > secondSolutionCost);
      }
      #endregion
   }
}