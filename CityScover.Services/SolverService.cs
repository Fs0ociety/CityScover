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
using System.Collections.ObjectModel;
using System.IO;

namespace CityScover.Services
{
   public class SolverService : Singleton<SolverService>, ISolverService
   {
      private ICollection<Configuration> _tourConfigurations = new Collection<Configuration>();

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
            _tourConfigurations.Add(config);
         }

         foreach (var tourConfig in _tourConfigurations)
         {
            Solver.Instance.Execute(tourConfig);
         }
      }
      #endregion
   }
}
