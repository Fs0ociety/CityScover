//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 27/11/2018
//

using CityScover.Commons;
using CityScover.Engine.Workers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.Engine
{
   internal class ToProblem : ProblemBase
   {
      #region Constants
      private const int PenaltyAmount = -200;
      #endregion

      #region Private fields
      private readonly DateTime _tMax;
      #endregion

      #region Constructors
      internal ToProblem()
      {
         var solverConfig = Solver.Instance.WorkingConfiguration;
         _tMax = solverConfig.ArrivalTime.Add(solverConfig.TourDuration);
         ObjectiveFunc = CalculateCost;
         PenaltyFunc = CalculatePenalty;
         IsMinimizing = false;

         Constraints.Add(
            new KeyValuePair<string, Func<ToSolution, bool>>(Utils.TMaxConstraint, IsTMaxConstraintSatisfied));
         Constraints.Add(
            new KeyValuePair<string, Func<ToSolution, bool>>(Utils.TimeWindowsConstraint, IsTimeWindowsConstraintSatisfied));
      }
      #endregion

      #region Overrides
      internal sealed override Func<ToSolution, int> ObjectiveFunc { get; }
      internal sealed override Func<ToSolution, int> PenaltyFunc { get; }
      #endregion

      #region Objective Function delegates
      /// <summary>
      /// Calculates the solution cost passed as parameter using an equation of convex combination.
      /// </summary>
      /// <param name="solution">
      /// Solution to evaluate.
      /// </param>
      /// <returns>
      /// An Evaluation Object.
      /// </returns>
      private int CalculateCost(ToSolution solution)
      {
         // Calcolo del termine del gradimento.
         int scoreTerm = solution.Tour.Nodes.Sum(node => node.Entity.Score.Value);

         // Il peso che determina l'importanza dei termini dell'equazione.
         double lambda = Solver.Instance.CurrentObjectiveFunctionWeight;

         // Calcolo del termine che da peso alla distanza tra il nodo corrente e quello precedente.
         // In particolare, vengono privilegati i nodi molto vicini con un peso molto alto
         // che darà quindi un maggior gradimento.
         double distanceTerm = solution.Tour.Nodes.Sum(node =>
         {
            RouteWorker edge = solution.Tour.GetEdges(node.Entity.Id).FirstOrDefault();
            if (edge is null)
            {
               return 0;
            }
            double distanceWeight = GetDistanceTermWeight(edge.Weight.Invoke());
            return distanceWeight * node.Entity.Score.Value;
         });

         return (int)Math.Round(lambda * scoreTerm + (1 - lambda) * distanceTerm);
      }

      private double GetDistanceTermWeight(double distanceNode)
      {
         double weight = default;
         if (distanceNode >= 0 && distanceNode <= 50)
         {
            weight = 12;
         }
         else if (distanceNode >= 51 && distanceNode <= 100)
         {
            weight = 10;
         }
         else if (distanceNode >= 101 && distanceNode <= 200)
         {
            weight = 9;
         }
         else if (distanceNode >= 201 && distanceNode <= 300)
         {
            weight = 8;
         }
         else if (distanceNode >= 301 && distanceNode <= 500)
         {
            weight = 7;
         }
         else if (distanceNode >= 501 && distanceNode <= 1000)
         {
            weight = 3.5;
         }
         else if (distanceNode >= 1001 && distanceNode <= 1500)
         {
            weight = 1;
         }
         else if (distanceNode > 1501)
         {
            weight = 0.25;
         }
             
         return weight;
      }
      #endregion

      #region Penalty Function delegates
      private int CalculatePenalty(ToSolution solution) => PenaltyAmount;
      #endregion

      #region Constraints delegates
      private bool IsTimeWindowsConstraintSatisfied(ToSolution solution)
      {
         bool satisfied = true;
         CityMapGraph solutionGraph = solution.Tour;
         using (var processingNodes = solutionGraph.TourPoints.GetEnumerator())
         {
            while (satisfied && processingNodes.MoveNext())
            {
               var node = processingNodes.Current;
               TimeSpan visitTime = default;

               if (node?.Entity.TimeVisit != null)
               {
                  visitTime = node.Entity.TimeVisit.Value;
               }
               if (node == null)
               {
                  continue;
               }

               DateTime nodeTime = node.ArrivalTime + node.WaitOpeningTime;
               foreach (var time in node.Entity.OpeningTimes)
               {
                  if (time.ClosingTime.HasValue && nodeTime > (time.ClosingTime.Value - visitTime))
                  {
                     satisfied = false;
                     //continue;
                  }
               }
            }
         }

         return satisfied;
      }

      private bool IsTMaxConstraintSatisfied(ToSolution solution)
      {
         return solution.TimeSpent <= _tMax;
      }
      #endregion
   }
}