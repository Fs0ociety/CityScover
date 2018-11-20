//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 20/11/2018
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
      /// Calculates the solution cost passed as parameter using an equation of convex combination.
      /// </summary>
      /// <param name="solution">
      /// Solution to evaluate.
      /// </param>
      /// <returns>
      /// An Evaluation Object.
      /// </returns>
      private int CalculateCost(TOSolution solution)
      {
         // Calcolo del termine del gradimento.
         int scoreTerm = solution.SolutionGraph.Nodes.Sum(node => node.Entity.Score.Value);

         // Il peso che determina l'importanza dei termini dell'equazione.
         double lambda = 0.5;

         // Se il grafo è un ciclo, qua tengo conto anche dell'arco di ritorno che va dall'ultimo POI
         // all'hotel. Se è un cammino, il termine non c'è e l'espressione funziona lo stesso.
         //double distanceWeightSum = default;
         //foreach (var edge in solution.SolutionGraph.Edges)
         //{
         //   distanceWeightSum += 1 / edge.Weight.Invoke();
         //}
         double distanceWeightSum = solution.SolutionGraph.Edges.Sum(edge => 1 / edge.Weight.Invoke());

         //Calcolo del termine della distanza.
         double distanceTerm = solution.SolutionGraph.Nodes.Sum(node =>
         {
            RouteWorker edge = solution.SolutionGraph.GetEdges(node.Entity.Id).FirstOrDefault();
            double nodeDistScoreTerm = node.Entity.Score.Value / edge.Weight.Invoke();
            return nodeDistScoreTerm / distanceWeightSum;
         });
         //double distanceTerm = default;
         //foreach (var node in solution.SolutionGraph.Nodes)
         //{
         //   RouteWorker edge = solution.SolutionGraph.GetEdges(node.Entity.Id).FirstOrDefault();
         //   double nodeDistScoreTerm = node.Entity.Score.Value / edge.Weight.Invoke();
         //   distanceTerm += nodeDistScoreTerm / distanceWeightSum;
         //}

         return (int)Math.Round((lambda * scoreTerm) + ((1 - lambda) * distanceTerm));
      }
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