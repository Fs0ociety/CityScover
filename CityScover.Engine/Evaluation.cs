//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 22/08/2018
//

namespace CityScover.Engine
{
   /// <summary>
   /// An Evaluation that represent a more complex estimate of a solution cost than the simple cost value.
   /// </summary>
   internal struct Evaluation
   {
      internal double Cost { get; set; }

      internal double Penalty { get; set; }
   }
}