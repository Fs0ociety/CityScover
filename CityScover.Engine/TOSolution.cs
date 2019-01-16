//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 16/01/2019
//

using CityScover.Engine.Workers;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   internal class ToSolution
   {
      #region Private fields
      private static int _sequenceId;
      private CityMapGraph _tour;
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
      /// Property used to assign a significative description of an instance of ToSolution class.
      /// </summary>
      internal string Description { get; set; }

      /// <summary>
      /// Property used to assign the algorithm's move which has produced an instance of ToSolution class.
      /// </summary>
      internal Tuple<int, int> Move { get; set; }

      /// <summary>
      /// This is the internal structure formed by nodes and edges of Solution.
      /// </summary>
      internal CityMapGraph Tour {
         get => _tour;
         set
         {
            _tour = value;
            if (_tour != null)
            {
               _tour.CalculateTimes();
               TimeSpent = CalculateTotalTime();
               _tour.TimeSpent = TimeSpent;
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

      #region Private methods
      private DateTime CalculateTotalTime()
      {
         var solver = Solver.Instance;
         InterestPointWorker startPoi = _tour.GetStartPoint();
         InterestPointWorker endPoi = _tour.GetEndPoint();
         DateTime endPoiTotalTime = endPoi.TotalTime;

         var returnEdge = solver.CityMapGraph.GetEdge(endPoi.Entity.Id, startPoi.Entity.Id);
         if (returnEdge is null)
         {
            throw new NullReferenceException(nameof(returnEdge));
         }

         double averageSpeedWalk = solver.WorkingConfiguration.WalkingSpeed;
         TimeSpan timeReturn = TimeSpan.FromSeconds(returnEdge.Weight() / averageSpeedWalk);
         DateTime timeSpent = endPoiTotalTime.Add(timeReturn);
         return timeSpent;
      }
      #endregion

      #region Internal static methods
      internal static string SolutionCollectionToString(IEnumerable<ToSolution> solutions)
      {
         var solver = Solver.Instance;
         string message = string.Empty;
         if (solutions is null || !solutions.Any())
         {
            return message;
         }

         message += $"\t{MessagesRepository.GetMessage(MessageCode.ALCompletionSummary)}\n\n";

         solutions.ToList().ForEach(solution =>
         {
            message += $"\t{MessagesRepository.GetMessage(MessageCode.TOSolutionCollectionId, solution.Id)} {solution.Tour.PrintGraph()}\n";
         });
         var bestSolution = solutions.Where(solution => solution.IsValid).MaxBy(solution => solution.Cost);
         message += $"\n\t{MessagesRepository.GetMessage(MessageCode.TOSolutionFinalTour, bestSolution.Id, bestSolution.Tour.PrintGraph())}";

         var tourDuration = bestSolution.Tour.GetEndPoint().TotalTime - solver.WorkingConfiguration.ArrivalTime;
         message += $"\n\t{MessagesRepository.GetMessage(MessageCode.TOSolutionTotalTimeAndValidity, bestSolution.Cost, tourDuration.Hours, tourDuration.Minutes, bestSolution.IsValid)}";
         message += $"\n\t{MessagesRepository.GetMessage(MessageCode.TOSolutionTotalDistance, bestSolution.Tour.GetTotalDistance() * 0.001)}";
         return message;
      }

      internal static void ResetSequenceId()
      {
         _sequenceId = 0;
      }
      #endregion
   }
}
