//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 13/12/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   internal class ToSolution
   {
      #region Private fields
      private static int _sequenceId;
      private CityMapGraph _solutionGraph;
      #endregion

      #region Constructors
      internal ToSolution()
      {
         Id = ++_sequenceId;
      }

      #endregion

      #region Internal properties
      /// <summary>
      /// Each Solution has an own ID.
      /// </summary>
      internal int Id { get; set; }

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
      internal IDictionary<string, bool> ProblemConstraints { get; } = new Dictionary<string, bool>();

      /// <summary>
      /// Property used to assign a significative description for this solution.
      /// </summary>
      internal string Description { get; set; }

      /// <summary>
      /// Property used to assign the algorithm move which has made this solution.
      /// </summary>
      internal Tuple<int, int> Move { get; set; }

      /// <summary>
      /// This is the internal structure formed by nodes and edges of Solution.
      /// </summary>
      internal CityMapGraph SolutionGraph {
         get => _solutionGraph;
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

      #region Internal methods
      internal string ViolatedConstraintsToString()
      {
         string message = MessagesRepository
            .GetMessage(MessageCode.ViolatedConstraints) + ":\n";

         foreach (var constraint in ProblemConstraints)
         {
            if (!constraint.Value)
            {
               message += MessagesRepository
                  .GetMessage(MessageCode.ViolatedConstraint, constraint.Key) + "\n";
            }
         }
         return message;
      }
      #endregion

      #region Internal static methods
      internal static string SolutionCollectionToString(IEnumerable<ToSolution> solutions)
      {
         string message = string.Empty;
         if (solutions is null || !solutions.Any())
         {
            return message;
         }

         message += $"{MessagesRepository.GetMessage(MessageCode.ALCompletionSummary)}\n";

         solutions.ToList().ForEach(solution =>
         {
            message += $"{MessagesRepository.GetMessage(MessageCode.TOSolutionCollectionId, solution.Id)} {solution.SolutionGraph.ToString()}\n";
         });
         ToSolution bestSolution = solutions
            .Aggregate((left, right) => left.Cost > right.Cost ? left : right);
         message += $"\n{MessagesRepository.GetMessage(MessageCode.TOSolutionFinalTour, bestSolution.Id, bestSolution.SolutionGraph.ToString())}";

         TimeSpan tourDuration = bestSolution.SolutionGraph.GetEndPoint().TotalTime - Solver.Instance.WorkingConfiguration.ArrivalTime;
         message += $"\n{MessagesRepository.GetMessage(MessageCode.TOSolutionTotalTimeAndValidity, bestSolution.Cost, tourDuration.Hours, tourDuration.Minutes, bestSolution.IsValid)}";
         return message;
      }

      internal static void ResetSequenceId()
      {
         _sequenceId = 0;
      }
      #endregion

      #region Private methods
      private DateTime CalculateTotalTime()
      {
         InterestPointWorker startPoi = _solutionGraph.GetStartPoint();
         InterestPointWorker endPoi = _solutionGraph.GetEndPoint();
         DateTime endPoiTotalTime = endPoi.TotalTime;

         RouteWorker returnEdge = Solver.Instance.CityMapGraph.GetEdge(endPoi.Entity.Id, startPoi.Entity.Id);
         if (returnEdge is null)
         {
            throw new NullReferenceException(nameof(returnEdge));
         }

         double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed;
         TimeSpan timeReturn = TimeSpan.FromSeconds(returnEdge.Weight() / averageSpeedWalk);
         DateTime timeSpent = endPoiTotalTime.Add(timeReturn);
         return timeSpent;
      }
      #endregion
   }
}
