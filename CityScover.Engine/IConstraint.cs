//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/08/2018
//

namespace CityScover.Engine
{
   /// <summary>
   /// This interface represents the abstraction of problem's constraint.
   /// </summary>
   internal interface IConstraint
   {
      byte Id { get; set; }
      bool IsSatisfied(Solution solution);
   }
}
