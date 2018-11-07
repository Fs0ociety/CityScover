//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 07/11/2018
//

using CityScover.ADT.Graphs;
using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Engine.Workers
{
   internal sealed class CityMapGraph : Graph<int, InterestPointWorker, RouteWorker>
   {
      #region Internal properties
      internal IEnumerable<InterestPointWorker> TourPoints => 
         Nodes.Where(node => node.Entity.Id != Solver.Instance.WorkingConfiguration.StartingPointId);      
      #endregion

      #region Internal methods
      internal void AddRouteFromGraph(CityMapGraph source, int fromPOIKey, int toPOIKey)
      {
         if (source is null)
         {
            throw new ArgumentNullException();
         }

         RouteWorker edge = source.GetEdge(fromPOIKey, toPOIKey);
         if (edge is null)
         {
            throw new InvalidOperationException();
         }

         AddEdge(fromPOIKey, toPOIKey, edge);
      }

      internal void AddNodeFromGraph(CityMapGraph source, int nodeKey)
      {
         if (ContainsNode(nodeKey))
         {
            return;
         }

         if (source is null)
         {
            throw new ArgumentNullException();
         }

         if (!source.ContainsNode(nodeKey))
         {
            throw new InvalidOperationException();
         }

         AddNode(nodeKey, source[nodeKey]);
      }

      internal InterestPointWorker GetStartPoint()
      {
         int startNodeId = Solver.Instance.WorkingConfiguration.StartingPointId;
         if (!ContainsNode(startNodeId))
         {
            throw new InvalidOperationException();
         }

         return Nodes
            .Where(x => x.Entity.Id == startNodeId)
            .FirstOrDefault();
      }

      internal InterestPointWorker GetEndPoint()
      {
         int startNodeId = Solver.Instance.WorkingConfiguration.StartingPointId;
         InterestPointWorker endPoint = default;
         foreach (var node in Nodes)
         {
            int adjNodeId = GetAdjacentNodes(node.Entity.Id).FirstOrDefault();
            if (adjNodeId == 0 || adjNodeId == startNodeId)
            {
               endPoint = node;
               break;
            }
         }

         return endPoint;
      }

      /// <summary>
      /// A deep copy of the current CityMapGraph object.
      /// </summary>
      /// <returns>A CityMapGraph object</returns>
      internal CityMapGraph DeepCopy()
      {
         CityMapGraph copy = new CityMapGraph();
         foreach (var node in Nodes)
         {
            InterestPointWorker copyInterestPointWorker = node.DeepCopy();
            copy.AddNode(node.Entity.Id, copyInterestPointWorker);
         }

         foreach (var node in Nodes)
         {
            var edges = GetEdges(node.Entity.Id);
            foreach (var edge in edges)
            {
               RouteWorker copyRouteWorker = edge.DeepCopy();
               copy.AddEdge(edge.Entity.PointFrom.Id, edge.Entity.PointTo.Id, copyRouteWorker);
            }
         }
         return copy;
      }

      internal void CalculateTimes()
      {
         int startPOIId = Solver.Instance.WorkingConfiguration.StartingPointId;
         DateTime currNodeTotalTime = Solver.Instance.WorkingConfiguration.ArrivalTime;
         DateTime currNodeArrivalTime = currNodeTotalTime;
         TimeSpan currNodeWaitOpeningTime = default;

         BreadthFirstSearch(startPOIId,
            (node, isVisited) => node.IsVisited = isVisited,
            node => { return node.IsVisited; },
            node =>
            {
               node.ArrivalTime = currNodeArrivalTime;
               node.TotalTime = currNodeTotalTime;
               node.WaitOpeningTime = currNodeWaitOpeningTime;
            },
            edge =>
            {
               double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed;
               TimeSpan timeWalk = TimeSpan.FromSeconds(edge.Weight() / averageSpeedWalk);

               InterestPoint edgeDestNode = edge.Entity.PointTo;
               TimeSpan visitTime = default;
               if (edgeDestNode.TimeVisit.HasValue)
               {
                  visitTime = edgeDestNode.TimeVisit.Value;
               }

               currNodeArrivalTime = currNodeTotalTime.Add(timeWalk);
               currNodeTotalTime = currNodeArrivalTime.Add(visitTime);
               
               // Se il vincolo delle Time Windows è rilassato, NON devo calcolare
               // il waitOpeningTime.. Però così con il 2 è cablato..
               if (Solver.Instance.ConstraintsToRelax.Contains(2))
               {
                  return;
               }

               Collection<TimeSpan> deltaOpeningTimes = new Collection<TimeSpan>();
               foreach (var time in edgeDestNode.OpeningTimes)
               {
                  if (!time.OpeningTime.HasValue || !time.ClosingTime.HasValue)
                  {
                     continue;
                  }

                  DateTime openingTime = time.OpeningTime.Value;
                  DateTime closingTime = time.ClosingTime.Value;
                  if (currNodeArrivalTime < openingTime)
                  {
                     TimeSpan currDeltaOpeningTime = openingTime.Subtract(currNodeArrivalTime);
                     deltaOpeningTimes.Add(currDeltaOpeningTime);
                  }
                  else if (currNodeArrivalTime > openingTime && currNodeArrivalTime < (closingTime - visitTime))
                  {
                     deltaOpeningTimes.Clear();
                     break;
                  }
               }
               if (deltaOpeningTimes.Count > 0)
               {
                  currNodeWaitOpeningTime = deltaOpeningTimes.Min();
                  currNodeTotalTime = currNodeTotalTime.Add(currNodeWaitOpeningTime);
               }
               else
               {
                  currNodeWaitOpeningTime = default;
               }
            });         
      }
      #endregion

      #region Overrides
      public override string ToString()
      {
         string result = String.Empty;
         int startPOIId = Solver.Instance.WorkingConfiguration.StartingPointId;
         BreadthFirstSearch(startPOIId,
            (node, isVisited) => node.IsVisited = isVisited,
            (node) => { return node.IsVisited; },
            node => result += $"({node.Entity.Name})",
            edge =>
            {
               if (edge.Entity.PointTo.Id != startPOIId)
               {
                  result += $" --> ";
               }
            });
         return result;
      } 
      #endregion
   }
}