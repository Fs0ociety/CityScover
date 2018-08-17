//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

using CityScover.Engine;
using CityScover.Utils;

namespace CityScover.Services
{
   public class SolverService : Singleton<SolverService>, ISolverService
   {
      public void Run()
      {
         IConfigurationService configService = ConfigurationService.Instance;
         Solver.Instance.Run(configService.CurrentConfiguration);
      }
   }
}
