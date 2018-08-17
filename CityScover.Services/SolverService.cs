//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

using CityScover.Engine;
using CityScover.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace CityScover.Services
{
   public class SolverService : Singleton<SolverService>, ISolverService
   {
      #region ISolverService implementations
      public void Run(IEnumerable<string> configurationFiles)
      {
         if (configurationFiles == null)
         {
            throw new ArgumentNullException(nameof(configurationFiles));
         }

         ICollection<string> configFiles = configurationFiles as ICollection<string>;
         if (configFiles == null)
         {
            throw new InvalidCastException();
         }

         if (configFiles.Count == 0)
         {
            throw new FileNotFoundException("The directory does not contain files.");
         }

         IConfigurationService configService = ConfigurationService.Instance;
         foreach (var configFile in configurationFiles)
         {
            Configuration config = configService.ReadConfigurationFromXml(configFile);
            Solver.Instance.Execute(config);
         }
      }
      #endregion
   }
}
