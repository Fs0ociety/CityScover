//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 25/08/2018
//

using CityScover.Engine.Workers;
using CityScover.Entities;
using CityScover.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Engine
{
   /// <summary>
   /// This class represent the Facade class of the CityScover.Engine, implemented as a Singleton.
   /// Contains the Execute method to run the configuration passed as argument.
   /// The Solver uses ExecutionTracer and SolverHelpers classes to do overall work.
   /// </summary>
   public sealed partial class Solver : Singleton<Solver>
   {
      private ICollection<Solution> _solutions;
      private Solution _bestSolution;

      #region Constructors
      private Solver()
      {
         // Il Solver crea il problema e lo trasmette all'ExecutionTracer.
         // Problem p = new Problem();
      }
      #endregion

      #region Internal properties

      /// <summary>
      /// Configuration to execute received from the SolverService.
      /// </summary>
      internal Configuration WorkingConfiguration { get; private set; }

      /// <summary>
      /// TODO
      /// </summary>
      internal Problem Problem { get; private set; }

      /// <summary>
      /// Points of interest filtered from the graph of the city.
      /// </summary>
      internal IEnumerable<InterestPoint> Points { get; private set; }

      /// <summary>
      /// Complete graph created which represents the graph of the city.
      /// This object is created by Solver.
      /// </summary>
      internal CityMapGraph CityMapGraph { get; private set; }

      /// <summary>
      /// All solutions derived from the execution of an Algorithm.
      /// </summary>
      internal IEnumerable<Solution> Solutions { get; private set; }

      internal Solution BestSolution { get; private set; }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         _solutions = new Collection<Solution>();
      }
      #endregion
   }
}