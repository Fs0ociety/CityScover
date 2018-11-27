//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 26/11/2018
//

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   /// <summary>
   /// This type represents the abstraction of a "Move" for the Tabu Search algorithm.
   /// Tabu Search uses a Tabu list that contains items of this type.
   /// </summary>
   internal class TabuMove
   {
      #region Constructors
      internal TabuMove(int firstEdgeId, int secondEdgeId, int expiration = 0)
      {
         FirstEdgeId = firstEdgeId;
         SecondEdgeId = secondEdgeId;
         Expiration = expiration;
      }
      #endregion

      #region Internal properties
      internal int FirstEdgeId { get; }
      internal int SecondEdgeId { get; }
      internal int Expiration { get; set; }
      #endregion   
   }
}