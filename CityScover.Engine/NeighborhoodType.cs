//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 11/10/2018
//

namespace CityScover.Engine
{
   #region Enumeration
   public enum NeighborhoodType
   {
      /// <summary>
      /// Invalid neighborhood type.
      /// </summary>
      None,

      /// <summary>
      /// Two Opt neighborhood type.
      /// </summary>
      TwoOpt,

      /// <summary>
      /// City Swap neighborhood type.
      /// </summary>
      CitySwap,

      /// <summary>
      /// Tabu Search neighborhood type.
      /// </summary>
      TabuSearchNeighborhood,

      // ...
      // Add your own new neighborhood's types here to expand the list.
   }
   #endregion
}