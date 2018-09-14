//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 14/09/2018
//

namespace CityScover.Engine.Algorithms.LocalSearch
{
   internal class TwoOptAlgorithm : Algorithm
   {
      internal override void OnError()
      {
         throw new System.NotImplementedException();
      }

      internal override void OnInitializing()
      {
         throw new System.NotImplementedException();
      }

      internal override void OnTerminated()
      {
         throw new System.NotImplementedException();
      }

      internal override void OnTerminating()
      {
         throw new System.NotImplementedException();
      }

      internal override void PerformStep()
      {
         //To test inner algorithm creation.         
         Algorithm algorithm = Solver.Instance.GetAlgorithm(AlgorithmType.LinKernighan);
         algorithm.Start();
      }

      internal override bool StopConditions()
      {
         throw new System.NotImplementedException();
      }
   }
}
