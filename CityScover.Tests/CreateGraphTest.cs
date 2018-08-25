﻿using System;
using System.Collections.Generic;
using CityScover.ADT.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityScover.Tests
{
   [TestClass]
   public class CreateGraphTest
   {
      #region Classes
      private class InterestPoint
      {
         public InterestPoint(string name)
         {
            Name = name;
         }

         public string Name { get; set; }
      }

      private class Route : IGraphEdgeWeight
      {
         public Route(double distance)
         {
            Distance = distance;
         }

         public double Distance { get; set; }

         public Func<double> Weight
         {
            get { return () => Distance; }
         }
      }

      private class CityMapGraph : Graph<int, InterestPoint, Route>
      {
      }

      #endregion

      [TestMethod]
      public void CreateGraph1()
      {
         CityMapGraph graph = new CityMapGraph();
         Assert.IsTrue(graph.NodeCount == 0);
         Assert.IsTrue(graph.EdgeCount == 0);

         InterestPoint p1 = new InterestPoint("P1");
         graph.AddNode(1, p1);
         graph.AddNode(2, new InterestPoint("P2"));
         graph.AddNode(3, new InterestPoint("P3"));
         graph.AddNode(4, new InterestPoint("P4"));
         graph.AddNode(5, new InterestPoint("P5"));
         graph.AddNode(6, new InterestPoint("P6"));
         Assert.IsTrue(graph.NodeCount == 6);

         graph.AddEdge(1, 2, new Route(100.0));
         graph.AddEdge(1, 4, new Route(200.0));
         graph.AddEdge(4, 2, new Route(300.0));
         graph.AddEdge(2, 5, new Route(400.0));
         graph.AddEdge(5, 4, new Route(500.0));
         graph.AddEdge(3, 5, new Route(600.0));
         graph.AddEdge(3, 6, new Route(700.0));
         graph.AddEdge(6, 6, new Route(0.0));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddEdge(17, 2, new Route(250.0)));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddEdge(1, 17, new Route(250.0)));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddEdge(1, 2, new Route(250.0)));
         Assert.IsTrue(graph.EdgeCount == 8);

         Assert.IsTrue(graph.ContainsNode(4));
         Assert.IsFalse(graph.ContainsNode(17));

         List<int> plist = new List<int>(graph.GetPredecessorNodes(5));
         Assert.IsTrue(plist.Count == 2);
         Assert.IsTrue(plist[0] == 2);
         Assert.IsTrue(plist[1] == 3);

         List<int> slist = new List<int>(graph.GetSuccessorNodes(5));
         Assert.IsTrue(slist.Count == 1);
         Assert.IsTrue(slist[0] == 4);
      }

      [TestMethod]
      public void CreateGraph2()
      {
         CityMapGraph graph = new CityMapGraph();
         Assert.IsTrue(graph.NodeCount == 0);
         Assert.IsTrue(graph.EdgeCount == 0);

         InterestPoint p1 = new InterestPoint("P1");
         graph.AddNode(1, p1);
         graph.AddNode(2, new InterestPoint("P2"));
         graph.AddNode(3, new InterestPoint("P3"));
         graph.AddNode(4, new InterestPoint("P4"));
         graph.AddNode(5, new InterestPoint("P5"));
         Assert.IsTrue(graph.NodeCount == 5);

         graph.AddUndirectEdge(1, 2, new Route(100.0));
         graph.AddUndirectEdge(1, 5, new Route(200.0));
         graph.AddUndirectEdge(2, 5, new Route(300.0));
         graph.AddUndirectEdge(2, 4, new Route(400.0));
         graph.AddUndirectEdge(2, 3, new Route(500.0));
         graph.AddUndirectEdge(3, 4, new Route(600.0));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddEdge(17, 2, new Route(250.0)));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddEdge(1, 17, new Route(250.0)));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddEdge(1, 2, new Route(250.0)));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddUndirectEdge(2, 2));
         Assert.IsTrue(graph.EdgeCount == 12);
      }

      [TestMethod]
      public void CreateGraph3()
      {
         CityMapGraph graph = new CityMapGraph();
         Assert.IsTrue(graph.NodeCount == 0);
         Assert.IsTrue(graph.EdgeCount == 0);

         graph.AddNode(1, new InterestPoint("P1"));
         graph.AddNode(2, new InterestPoint("P2"));
         graph.AddNode(3, new InterestPoint("P3"));
         graph.AddNode(4, new InterestPoint("P4"));
         graph.AddNode(5, new InterestPoint("P5"));
         Assert.IsTrue(graph.NodeCount == 5);

         graph.AddUndirectEdge(1, 2);
         graph.AddUndirectEdge(1, 5);
         graph.AddUndirectEdge(2, 4);
         graph.AddUndirectEdge(4, 3);
         graph.AddUndirectEdge(5, 3);
         Assert.IsTrue(graph.EdgeCount == 10);

         //graph.AddEdge(1, 2);
         //graph.AddEdge(2, 1);
         //graph.AddEdge(1, 5);
         //graph.AddEdge(5, 1);
         //graph.AddEdge(2, 4);
         //graph.AddEdge(4, 2);
         //graph.AddEdge(5, 3);
         //graph.AddEdge(3, 5);
         //graph.AddEdge(3, 4);
         //graph.AddEdge(4, 3);
         //Assert.IsTrue(graph.EdgeCount == 10);
                  
         int P1Grade = graph.GetNodeGrade(1);
         Assert.IsTrue(P1Grade == 2);
      }
   }
}