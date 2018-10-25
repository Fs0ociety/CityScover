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
using System.Threading.Tasks;

namespace CityScover.Services
{
   public interface ISolverService
   {
      /// <summary>
      /// Invoke the Solver instance of CityScover.Engine to run configurations contained into RunningConfigs.
      /// </summary>      
      Task Run();

      /// <summary>
      /// Invoke the Solver instance of CityScover.Engine to run the configuration passed as parameter.
      /// </summary>
      /// <param name="configuration">Configuration to execute.</param>
      Task Run(Configuration configuration);

      /// <summary>
      /// Invoke the Solver instance to display results at the end of Configuration's execution.
      /// </summary>
      void ReportConfiguration();
   }
}
