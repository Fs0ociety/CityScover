﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 28/10/2018
//

using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CityScover.Engine
{
   internal class TOProblem : ProblemBase
   {
      #region Constants
      private const int PenaltyAmount = 100;
      #endregion

      #region Private fields
      private readonly DateTime _tMax;
      private Func<TOSolution, int> _objectiveFunc;
      private Func<TOSolution, int> _penaltyFunc;
      #endregion

      #region Constructors
      internal TOProblem()
         : base()
      {
         var solverConfig = Solver.Instance.WorkingConfiguration;
         _tMax = solverConfig.ArrivalTime.Add(solverConfig.TourDuration);
         ObjectiveFunc = CalculateCost;
         PenaltyFunc = CalculatePenalty;
         IsMinimizing = false;

         Constraints.Add(
            new KeyValuePair<byte, Func<TOSolution, bool>>(1, IsTMaxConstraintSatisfied));
         Constraints.Add(
            new KeyValuePair<byte, Func<TOSolution, bool>>(2, IsTimeWindowsConstraintSatisfied));
      }
      #endregion

      #region Overrides
      internal override Func<TOSolution, int> ObjectiveFunc
      {
         get => _objectiveFunc;
         set
         {
            if (value != _objectiveFunc)
            {
               _objectiveFunc = value;
            }
         }
      }

      internal override Func<TOSolution, int> PenaltyFunc
      {
         get => _penaltyFunc;
         set
         {
            if (value != _penaltyFunc)
            {
               _penaltyFunc = value;
            }
         }
      }
      #endregion

      #region Objective Function delegates
      /// <summary>
      /// Calculates the solution cost passed as parameter.
      /// </summary>
      /// <param name="solution">
      /// Solution to evaluate.
      /// </param>
      /// <returns>
      /// An Evaluation Object.
      /// </returns>
      private int CalculateCost(TOSolution solution)
      {
         var solutionNodes = solution.SolutionGraph.Nodes;

         var totalScoreNodes = (from node in solutionNodes
                                select node.Entity.Score.Value).Sum();

         //int totalScoreEdges = default;
         //solutionNodes.ToList().ForEach(node =>
         //{
         //   var edges = solution.SolutionGraph.GetEdges(node.Entity.Id);
         //   try
         //   {
         //      var edge = edges.FirstOrDefault();
         //      if (edge != null)
         //      {
         //         totalScoreEdges += checked((int)edge.Weight());
         //      }
         //   }
         //   catch (OverflowException ex)
         //   {
         //      // PROVVISORIO
         //      Debug.WriteLine(
         //         $"Objective Function exception during conversion double to int {ex.Message}");
         //   }
         //});

         //return totalScoreNodes + totalScoreEdges;
         return totalScoreNodes;
      }
      #endregion

      #region Penalty Function delegates
      private int CalculatePenalty(TOSolution solution)
      {
         return PenaltyAmount;
      }
      #endregion

      #region Constraints delegates
      private bool IsTimeWindowsConstraintSatisfied(TOSolution solution)
      {
         bool satisfied = true;
         CityMapGraph solutionGraph = solution.SolutionGraph;
         int startPOIId = Solver.Instance.WorkingConfiguration.StartingPointId;
         IEnumerator<InterestPointWorker> processingNodes = solutionGraph.TourPoints.GetEnumerator();

         while (satisfied && processingNodes.MoveNext())
         {
            InterestPointWorker node = processingNodes.Current;
            TimeSpan visitTime = default;
            if (node.Entity.TimeVisit.HasValue)
            {
               visitTime = node.Entity.TimeVisit.Value;
            }

            DateTime nodeTime = node.ArrivalTime + node.WaitOpeningTime;
            foreach (var time in node.Entity.OpeningTimes)
            {
               if (time.ClosingTime.HasValue && nodeTime > (time.ClosingTime.Value - visitTime))
               {
                  satisfied = false;
                  continue;
               }
            }
         }
         return satisfied;
      }

      private bool IsTMaxConstraintSatisfied(TOSolution solution)
      {
         return solution.TimeSpent <= _tMax;
      }
      #endregion
   }
}