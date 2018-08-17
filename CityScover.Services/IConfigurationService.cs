//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

using CityScover.Engine;

namespace CityScover.Services
{
   public interface IConfigurationService
   {
      Configuration CurrentConfiguration { get; }
      void ReadConfigurationFromXML(string filename);
      void ReadConfigurationPath(string configsPath);
   }
}
