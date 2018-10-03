//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 03/10/2018
//

using CityScover.Engine.Workers;
using System.Threading.Tasks;

namespace CityScover.Engine.Algorithms.VariableDepthSearch
{
   internal class LinKernighanAlgorithm : Algorithm
   {
      private CityMapGraph _cityMapClone;
      private TOSolution _bestSolution;
      private byte _executedSteps;

      #region Constructors
      internal LinKernighanAlgorithm(byte steps)
         : this(steps, null)
      {         
      }

      internal LinKernighanAlgorithm(byte steps, AlgorithmTracker tracker)
         : base(tracker)
      {
         MaxSteps = steps;
      }
      #endregion

      #region Internal properties
      internal byte MaxSteps { get; private set; } 
      #endregion

      #region Overrides
      internal override void OnError()
      {
         throw new System.NotImplementedException();
      }

      internal override void OnInitializing()
      {
         _cityMapClone = Solver.CityMapGraph.DeepCopy();
         // TODO: la prende dal solver?
         _bestSolution = Solver.BestSolution;
      }

      internal override void OnTerminated()
      {
         throw new System.NotImplementedException();
      }

      internal override void OnTerminating()
      {
         // TODO: confronto finale per vedere se effettivamente ho incrementato qualcosa
         // mi verrebbe da farlo qua.
         throw new System.NotImplementedException();
      }

      internal override async Task PerformStep()
      {
         throw new System.NotImplementedException();
      }

      internal override bool StopConditions()
      {
         return _executedSteps == MaxSteps;
      } 
      #endregion
   }
}
