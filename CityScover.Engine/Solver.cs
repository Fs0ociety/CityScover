//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 22/08/2018
//

using CityScover.Engine.Workers;
using CityScover.Utils;
using System;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represent the Facade class of the CityScover.Engine, implemented as a Singleton.
   /// Contains the Execute method to run the configuration passed as argument.
   /// The Solver uses ExecutionTracer and SolverHelpers classes to do overall work.
   /// </summary>
   public sealed class Solver : Singleton<Solver>
   {
      #region Constructors
      private Solver()
      {
         // Il Solver crea il problema e lo trasmette all'ExecutionTracer.
         //Problem p = new Problem();
      }
      #endregion

      #region Internal properties
      internal Configuration WorkingConfiguration { get; private set; }
      internal CityMapGraph CityMapGraph { get; private set; }
      #endregion

      #region Private methods
      /// <summary>
      /// Initalize the graph of the city using CityScover.Data assembly.
      /// </summary>
      /// <returns></returns>
      private CityMapGraph CreateCityGraph()
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Public methods
      public void Initialize()
      {
         CityMapGraph = CreateCityGraph();
      }

      public void Execute(Configuration configuration)
      {
         InitializeWorkingConfig();
         Initialize();

         void InitializeWorkingConfig()
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
