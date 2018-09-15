//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 15/09/2018
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
   }
}
