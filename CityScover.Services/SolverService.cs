//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/09/2018
//

using CityScover.Commons;
using CityScover.Engine;
using CityScover.Engine.Configs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CityScover.Services
{
   public class SolverService : Singleton<SolverService>, ISolverService
   {
      private ICollection<Configuration> _tourConfigurations;

      #region Constructors
      private SolverService()
      {
      }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         _tourConfigurations = new Collection<Configuration>();
      }
      #endregion

      #region ISolverService implementations
      public async Task Run()
      {
         _tourConfigurations = RunningConfigs.Configurations;

         var solver = Solver.Instance;
         foreach (var tourConfig in _tourConfigurations)
         {
            await solver.Execute(tourConfig);
            //ReportConfiguration();
         }
      }

      public void ReportConfiguration()
      {
         // Usa stampe ordinate. Valutare un eventuale ReportService.
         throw new System.NotImplementedException();
      }
      #endregion
   }
}
