//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

using CityScover.Services;
using System.Configuration;

namespace CityScover
{
   class Program
   {
      static void Main(string[] args)
      {
         InitializeRepository();
         InitializeConfigurations();

         /*
          * Per ogni file di configurazione nella cartella delle configurazioni indicata da configsPath
          *    1. Leggo il file XML
          *    2. Setto la configurazione corrente di quel file XML
          *    3. Invoco il Solver con la configurazione corrente.
          */
      }

      private static void InitializeRepository()
      {
         ITourService tourService = TourService.Instance;
         //var points = tourService.Points;
         //var routes = tourService.Routes;
      }

      private static void InitializeConfigurations()
      {
         var configPaths = ConfigurationManager.AppSettings["configPaths"];
         IConfigurationService configService = ConfigurationService.Instance;
         ISolverService solverService = SolverService.Instance;

         configService.ReadConfigurationPath(configPaths);
         solverService.Run();
      }
   }
}
