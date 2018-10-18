using System;
using System.Collections.Generic;
using System.Linq;
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
         public bool IsVisited { get; set; }
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

         graph.AddUndirectedEdge(1, 2, new Route(100.0));
         graph.AddUndirectedEdge(1, 5, new Route(200.0));
         graph.AddUndirectedEdge(2, 5, new Route(300.0));
         graph.AddUndirectedEdge(2, 4, new Route(400.0));
         graph.AddUndirectedEdge(2, 3, new Route(500.0));
         graph.AddUndirectedEdge(3, 4, new Route(600.0));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddEdge(17, 2, new Route(250.0)));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddEdge(1, 17, new Route(250.0)));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddEdge(1, 2, new Route(250.0)));
         Assert.ThrowsException<InvalidOperationException>(() => graph.AddUndirectedEdge(2, 2));
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

         //graph.AddUndirectedEdge(1, 2);
         //graph.AddUndirectedEdge(1, 5);
         //graph.AddUndirectedEdge(2, 4);
         //graph.AddUndirectedEdge(4, 3);
         //graph.AddUndirectedEdge(5, 3);
         //Assert.IsTrue(graph.EdgeCount == 10);

         graph.AddEdge(1, 2);
         graph.AddEdge(2, 5);
         graph.AddEdge(5, 4);
         graph.AddEdge(4, 3);
         graph.AddEdge(3, 1);
         Assert.IsTrue(graph.EdgeCount == 5);

         var P1Neighbors = graph.GetAdjacentNodes(1);
         Assert.IsTrue(P1Neighbors.Count() == 1);
      }

      [TestMethod]
      public void CreateGraph4()
      {
         CityMapGraph graph = new CityMapGraph();
         graph.AddNode(1, new InterestPoint("P1"));
         graph.AddNode(2, new InterestPoint("P2"));
         graph.AddNode(3, new InterestPoint("P3"));
         graph.AddNode(4, new InterestPoint("P4"));

         graph.AddEdge(1, 2, new Route(100.0));
         graph.AddEdge(2, 3, new Route(200.0));
         graph.AddEdge(3, 4, new Route(300.0));
         graph.AddEdge(4, 1, new Route(400.0));
         
         Assert.IsTrue(graph.AreAdjacentEdges(1, 2, 2, 3));
         Assert.IsFalse(graph.AreAdjacentEdges(1, 2, 3, 4));
         Assert.IsFalse(graph.AreAdjacentEdges(1, 2, 3, 2));
         Assert.IsTrue(graph.AreAdjacentEdges(1, 2, 4, 1));

         IEnumerable<Route> edges = graph.Edges;

         IEnumerable<Route> nodeEdges = graph.GetEdges(1);

         CityMapGraph graph2 = new CityMapGraph();

         graph2.AddNode(1, new InterestPoint("P1"));
         graph2.AddNode(2, new InterestPoint("P2"));
         graph2.AddNode(3, new InterestPoint("P3"));
         graph2.AddNode(4, new InterestPoint("P4"));
         graph2.AddNode(5, new InterestPoint("P5"));
         graph2.AddNode(6, new InterestPoint("P6"));
         graph2.AddNode(7, new InterestPoint("P7"));
         graph2.AddNode(8, new InterestPoint("P8"));

         graph2.AddEdge(1, 2, new Route(10.0));
         graph2.AddEdge(2, 4, new Route(10.0));
         graph2.AddEdge(4, 3, new Route(10.0));
         graph2.AddEdge(3, 5, new Route(10.0));
         graph2.AddEdge(5, 6, new Route(10.0));
         graph2.AddEdge(6, 7, new Route(10.0));
         graph2.AddEdge(7, 8, new Route(10.0));

         string prova = $"PROVA: ";
         IEnumerable<InterestPoint> visit = graph2.BreadthFirstSearch(1,
            (node, isVisited) => node.IsVisited = isVisited,
            (node) => { return node.IsVisited; },
            node => prova += node.Name,
            edge => prova += " -- ");
      }
   }
}
