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
      internal sealed override Func<ToSolution, int> ObjectiveFunc { get; set; }
      internal sealed override Func<ToSolution, int> PenaltyFunc { get; set; }
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
         int scoreTerm = solution.SolutionGraph.Nodes.Sum(node => node.Entity.Score.Value);

         // Il peso che determina l'importanza dei termini dell'equazione.
         double lambda = Solver.Instance.CurrentObjectiveFunctionWeight;

         // Se il grafo è un ciclo, qua tengo conto anche dell'arco di ritorno che va dall'ultimo POI
         // all'hotel. Se è un cammino, il termine non c'è e l'espressione funziona lo stesso.
         double distanceWeightSum = solution.SolutionGraph.Edges.Sum(edge => 1 / edge.Weight.Invoke());

         //Calcolo del termine della distanza.
         double distanceTerm = solution.SolutionGraph.Nodes.Sum(node =>
         {
            RouteWorker edge = solution.SolutionGraph.GetEdges(node.Entity.Id).FirstOrDefault();
            if (edge is null)
            {
               return 0;
            }
            double nodeDistScoreTerm = node.Entity.Score.Value / edge.Weight.Invoke();
            return nodeDistScoreTerm / distanceWeightSum;
         });

         return (int)Math.Round((lambda * scoreTerm) + ((1 - lambda) * distanceTerm));
      }
      #endregion

      #region Penalty Function delegates
      private int CalculatePenalty(ToSolution solution) => PenaltyAmount;
      #endregion

      #region Constraints delegates
      private bool IsTimeWindowsConstraintSatisfied(ToSolution solution)
      {
         bool satisfied = true;
         CityMapGraph solutionGraph = solution.SolutionGraph;
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