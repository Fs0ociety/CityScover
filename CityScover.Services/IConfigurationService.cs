//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 16/10/2018
//

using CityScover.Engine;
using System.Collections.Generic;

namespace CityScover.Services
{
   public interface IConfigurationService
   {
      ICollection<Configuration> Configurations { get; }
      void ShowConfigurationsMenu();
      Configuration CreateConfiguration(params object[] configParams);
   }
}
