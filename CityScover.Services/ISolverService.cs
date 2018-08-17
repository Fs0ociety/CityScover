//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

namespace CityScover.Services
{
   public interface ISolverService
   {
      /// <summary>
      /// Invoke the Solver instance of CityScover.Engine to run the configuration passed as argument.
      /// </summary>
      /// <param name="configurationFile"></param>
      void Run(string configsPath);
   }
}
