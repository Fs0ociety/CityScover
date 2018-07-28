//
// CityScover
// Version 1.0
//
// Authors: Riccardo Mariotti
// File update: 28/07/2018
//

using System;

namespace CityScover.ADT.Graphs
{
   public interface IGraphEdgeWeight
   {
      Func<double> Weight { get; }
   }
}
