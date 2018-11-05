//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 04/11/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;

namespace CityScover.Engine
{
   internal class TOSolution
   {
      #region Private fields
      private static int _sequenceId;
      private CityMapGraph _solutionGraph;
      #endregion

      #region Constructors
      internal TOSolution()
      {
         Id = ++_sequenceId;
      }

      #endregion

      #region Internal properties
      /// <summary>
      /// Each Solution has an own ID.
      /// </summary>
      internal int Id { get; }

      /// <summary>
      /// Property used from SolverEvaluator to set a Cost for the Solution.
      /// </summary>
      internal int Cost { get; set; }      

      /// <summary>
      /// Property used from SolverEvaluator to set a Penalty for the Solution.
      /// </summary>
      internal int Penalty { get; set; }
      
      /// <summary>
      /// Property used from SolverValidator to check the TMax constraint.
      /// </summary>
      internal DateTime TimeSpent { get; private set; }

      /// <summary>
      /// Property used from SolverValidator for analysis of problem's constraints.
      /// Contains the results of validation of constraints.
      /// </summary>
      internal IDictionary<byte, bool> ProblemConstraints { get; set; } = new Dictionary<byte, bool>();

      /// <summary>
      /// This is the internal structure formed by nodes and edges of Solution.
      /// </summary>
      internal CityMapGraph SolutionGraph {
         get
         {
            return _solutionGraph;
         }
         set
         {
            _solutionGraph = value;
            if (_solutionGraph != null)
            {
               _solutionGraph.CalculateTimes();
               TimeSpent = CalculateTotalTime();
            }
         }
      }

      internal bool IsValid => Penalty == 0;
      #endregion

      #region Private methods
      private DateTime CalculateTotalTime()
      {
         InterestPointWorker startPOI = _solutionGraph.GetStartPoint();
         InterestPointWorker endPOI = _solutionGraph.GetEndPoint();
         DateTime endPOITotalTime = endPOI.TotalTime;

         RouteWorker returnEdge = Solver.Instance.CityMapGraph.GetEdge(endPOI.Entity.Id, startPOI.Entity.Id);
         if (returnEdge is null)
         {
            throw new NullReferenceException(nameof(returnEdge));
         }

         double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed;
         TimeSpan timeReturn = TimeSpan.FromSeconds(returnEdge.Weight() / averageSpeedWalk);
         DateTime timeSpent = endPOITotalTime.Add(timeReturn);
         return timeSpent;
      } 
      #endregion
   }
}
