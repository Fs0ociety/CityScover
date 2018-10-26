//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 20/10/2018
//

using CityScover.ADT.Graphs;
using CityScover.Entities;
using System;
using System.Collections.Generic;
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
         if (source == null)
         {
            throw new ArgumentNullException();
         }

         RouteWorker edge = source.GetEdge(fromPOIKey, toPOIKey);
         if (edge == null)
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

         if (source == null)
         {
            throw new ArgumentNullException();
         }

         if (!source.ContainsNode(nodeKey))
         {
            throw new InvalidOperationException();
         }

         AddNode(nodeKey, source[nodeKey]);
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
         DateTime currNodeArrivalTime = default;
         TimeSpan currNodeWaitOpeningTime = default;

         BreadthFirstSearch(startPOIId,
            (node, isVisited) => node.IsVisited = isVisited,
            node => { return node.IsVisited; },
            node =>
            {
               if (node.Entity.Id == startPOIId)
               {
                  return;
               }

               node.ArrivalTime = currNodeArrivalTime;
               node.WaitOpeningTime = currNodeWaitOpeningTime;
            },
            edge =>
            {
               double averageSpeedWalk = Solver.Instance.WorkingConfiguration.WalkingSpeed / 60.0;
               TimeSpan timeWalk = TimeSpan.FromMinutes(edge.Weight() / averageSpeedWalk);
               currNodeArrivalTime = currNodeArrivalTime.Add(timeWalk);

               TimeSpan deltaOpeningTime = default;
               InterestPoint destNodeEdge = edge.Entity.PointTo;
               foreach (var time in destNodeEdge.OpeningTimes)
               {
                  if (!time.OpeningTime.HasValue)
                  {
                     continue;
                  }

                  DateTime openingTime = time.OpeningTime.Value;
                  if (currNodeArrivalTime < openingTime)
                  {
                     TimeSpan currDeltaOpeningTime = openingTime.Subtract(currNodeArrivalTime);
                     if (currDeltaOpeningTime < deltaOpeningTime)
                     {
                        deltaOpeningTime = currDeltaOpeningTime;
                     }
                  }
               }
               currNodeWaitOpeningTime = deltaOpeningTime;
            });         
      }
      #endregion
   }
}
