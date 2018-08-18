//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/08/2018
//

using CityScover.Engine;

namespace CityScover.Services
{
   public interface IConfigurationService
   {          
      /// <summary>
      /// Parses the XML file passed as argument.
      /// </summary>
      /// <param name="filename"></param>
      Configuration ReadConfigurationFromXml(string filename);
   }
}
