//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/08/2018
//

using CityScover.Data;
using CityScover.Entities;
using CityScover.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represent the Facade class of the CityScover.Engine, implemented as a Singleton.
   /// Contains the Execute method to run the configuration passed as argument.
   /// The Solver uses ExecutionTracer and SolverHelpers classes to do overall work.
   /// </summary>
   public sealed partial class Solver : Singleton<Solver>
   {
      #region Public methods
      public void Initialize()
      {
         CityScoverRepository.LoadPointsFileByValue(WorkingConfiguration.PointsCount);
         var points = CityScoverRepository.Points;

         // NOTA
         // Nell'entita' TourCategory dell'assembly CityScover.Entities modificare la property Id della categoria del tour da int nullable a tipo TourCategoryType.
         var pointCategory = from point in points where point.Category.Id == (int)WorkingConfiguration.TourCategory select point;
         var pointCategory2 = points.Where(x => x.Category.Id == (int)WorkingConfiguration.TourCategory);

         RoutesGenerator.GenerateRoutes((ICollection<InterestPoint>)points, WorkingConfiguration.PointsCount);
         CityScoverRepository.LoadRoutes(WorkingConfiguration.PointsCount);
         var routes = (ICollection<Route>)CityScoverRepository.Routes;
      }

      public void Execute(Configuration configuration)
      {
         SetWorkingConfig();
         Initialize();

         void SetWorkingConfig()
         {
            WorkingConfiguration = configuration;
            SolverConstraintHelper.Instance.WorkingConfiguration = configuration;
            SolverEvaluationHelper.Instance.WorkingConfiguration = configuration;
         }

         throw new NotImplementedException(nameof(Execute));
      }
      #endregion
   }
}
