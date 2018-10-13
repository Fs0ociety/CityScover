//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 13/10/2018
//

using CityScover.Engine.Workers;
using System;

namespace CityScover.Engine.Algorithms.Metaheuristics
{
   /// <summary>
   /// This type represents the abstraction of a "Move" for the Tabu Search algorithm.
   /// Tabu Search uses a Tabu list that contains items of this type.
   /// </summary>
   internal class TabuMove : IEquatable<TabuMove>
   {
      #region Constructors
      internal TabuMove(RouteWorker firstEdge, RouteWorker secondEdge, int expiration = 0)
      {
         FirstEdge = firstEdge ?? throw new ArgumentNullException(nameof(firstEdge));
         SecondEdge = secondEdge ?? throw new ArgumentNullException(nameof(secondEdge));
         Expiration = expiration;
      }
      #endregion

      #region Internal properties
      internal RouteWorker FirstEdge { get; set; }
      internal RouteWorker SecondEdge { get; set; }
      internal int Expiration { get; set; }
      #endregion

      #region IEquatable implementation
      public bool Equals(TabuMove other)
      {
         return FirstEdge == other.FirstEdge &&
            SecondEdge == other.SecondEdge;
      }
      #endregion
   }
}