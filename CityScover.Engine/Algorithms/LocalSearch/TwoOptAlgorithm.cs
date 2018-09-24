//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/09/2018
//

using System.Threading.Tasks;

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
         var solver = Solver.Instance;
         var childrenFlows = solver.CurrentStage.Flow.ChildrenFlows;
         
         // Inner algorithm creation test.
         foreach (var child in childrenFlows)
         {
            Algorithm algorithm = solver.GetAlgorithm(child.CurrentAlgorithm);
            for (int i = 0; i < child.RunningTimes; i++)
            {
               // Code reuse of ExecuteWithMonitoring from Solver.
               solver.ExecutionInternalFunc.Invoke(algorithm);
            }
         }
      }

      internal override bool StopConditions()
      {
         throw new System.NotImplementedException();
      }
   }
}
