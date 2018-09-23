//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 23/09/2018
//

using System;
using System.Collections.Generic;

namespace CityScover.Engine.Algorithms
{
   //TODO: Di fatto è uno Strategy Pattern, ma il metodo GetAllMoves produce un'insieme di soluzioni.
   //Si può mantenere così oppure la GetAllMoves produce un'insieme di soluzioni internamente, poi accessibili
   //con una proprietà get ?
   internal abstract class TONeighborhood
   {
      internal TOSolution GetBest(IEnumerable<TOSolution> neighborhood, TOSolution currentSolution, byte? maxImprovementsCount)
      {
         if (neighborhood == null || currentSolution == null)
         {
            throw new ArgumentNullException(nameof(TONeighborhood));
         }

         if (maxImprovementsCount.HasValue && maxImprovementsCount == 0)
         {
            throw new ArgumentException("Se maxImprovementsCount è valorizzato, non può avere valore 0.");
         }

         TOSolution bestSolution = currentSolution;
         byte currentImprovement = default;

         foreach (var solution in neighborhood)
         {
            if (maxImprovementsCount.HasValue && currentImprovement > maxImprovementsCount)
            {
               break;
            }
            
            if (solution.Cost < bestSolution.Cost)
            {
               bestSolution = solution;
               currentImprovement++;
            }
         }

         return bestSolution;
      }

      internal abstract IEnumerable<TOSolution> GetAllMoves(TOSolution currentSolution);   
   }
}
