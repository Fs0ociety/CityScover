﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 14/11/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   internal class TOProblem : ProblemBase
   {
      #region Constants
      private const int PenaltyAmount = -200;
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
            new KeyValuePair<string, Func<TOSolution, bool>>(Utils.TMaxConstraint, IsTMaxConstraintSatisfied));
         Constraints.Add(
            new KeyValuePair<string, Func<TOSolution, bool>>(Utils.TimeWindowsConstraint, IsTimeWindowsConstraintSatisfied));
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
      private int CalculateCost(TOSolution solution) => 
         solution.SolutionGraph.Nodes.Sum(node => node.Entity.Score.Value);
      #endregion

      #region Penalty Function delegates
      private int CalculatePenalty(TOSolution solution) => PenaltyAmount;
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