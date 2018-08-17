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

namespace CityScover.Services
{
   public class ConfigurationService : Singleton<ConfigurationService>, IConfigurationService
   {
      private Configuration _currentConfig;

      public Configuration CurrentConfiguration
      {
         get => _currentConfig;
         private set => _currentConfig = value;
      }

      #region Public methods
      public void ReadConfigurationFromXML(string filename)
      {
         Configuration config = new Configuration();
         CurrentConfiguration = config;
      }

      public void ReadConfigurationPath(string configsPath)
      {
         if (configsPath == null)
         {
            throw new ArgumentNullException(nameof(configsPath));
         }

         // TODO
      }
      #endregion
   }
}
