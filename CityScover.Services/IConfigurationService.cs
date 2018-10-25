//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 25/10/2018
//

using CityScover.Engine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityScover.Services
{
   public interface IConfigurationService
   {
      ICollection<Configuration> Configurations { get; }
      Task ShowConfigurationsMenu();
      Configuration CreateConfiguration();
   }
}
