//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 04/10/2018
//

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
      /// Invoke the Solver instance to display results at the end of Configuration's execution.
      /// </summary>
      void ReportConfiguration();
   }
}
