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
using System.IO;

namespace CityScover.Services
{
   public class SolverService : Singleton<SolverService>, ISolverService
   {
      #region ISolverService implementations
      public void Run(string configsPath)
      {
         if (configsPath == null)
         {
            throw new ArgumentNullException(nameof(configsPath));
         }

         if (!Directory.Exists(configsPath))
         {
            throw new DirectoryNotFoundException(nameof(configsPath));
         }

         string[] filenames = Directory.GetFiles(configsPath);         
         if (filenames.Length == 0)
         {
            throw new FileNotFoundException("The directory does not contain files.");
         }

         IConfigurationService configService = ConfigurationService.Instance;
         foreach (var configFile in filenames)
         {
            Configuration config = configService.ReadConfigurationFromXml(configFile);
            Solver.Instance.Execute(config);
         }
      }
      #endregion
   }
}
