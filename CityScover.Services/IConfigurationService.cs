//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

namespace CityScover.Services
{
   public interface IConfigurationService
   {
      Configuration ReadConfigFromXml(string path);
   }
}
