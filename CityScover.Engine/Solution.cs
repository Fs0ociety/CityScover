//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/08/2018
//

using System.Collections.Generic;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal class Solution
   {
      /*
       * NOTA
       * Valutare se rendere tale classe astratta, quindi ereditare da Solution delle
       * classi concrete che descrivano la soluzione ad uno specifico problema.
       * 
       * (Ad esempio: vedi soluzione con lista di nodi per gli algoritmi Greedy e LS, 
       * o soluzione con lista di archi per l'algoritmo Lin Kernighan)
       */
      #region Internal properties
      internal IDictionary<byte, bool> ProblemConstraints { get; set; }
      internal double Cost { get; set; }
      #endregion
   }
}
