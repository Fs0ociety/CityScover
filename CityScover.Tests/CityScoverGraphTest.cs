using CityScover.ADT.Graphs;
using CityScover.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CityScover.Tests
{
   [TestClass]
   public class CityScoverGraphTest
   {
      //#region Classes
      //private class CityMapGraph : Graph<int, InterestPoint, Route>
      //{
      //}
      //#endregion

      //[TestMethod]
      //public void CreateGraph1()
      //{
      //   CityMapGraph graph = new CityMapGraph();
      //   Assert.IsTrue(graph.NodeCount == 0);
      //   Assert.IsTrue(graph.EdgeCount == 0);

      //   InterestPoint p1 = new InterestPoint();
      //   InterestPoint p2 = new InterestPoint();
      //   InterestPoint p3 = new InterestPoint();
      //   InterestPoint p4 = new InterestPoint();
      //   InterestPoint p5 = new InterestPoint();

      //   p1.Name = "P1";
      //   p2.Name = "P2";
      //   p3.Name = "P3";
      //   p4.Name = "P4";
      //   p5.Name = "P5";

      //   p1.Score = new ThematicScore();
      //   p2.Score = new ThematicScore();
      //   p3.Score = new ThematicScore();
      //   p4.Score = new ThematicScore();
      //   p5.Score = new ThematicScore();

      //   p1.Score.Value = 50;
      //   p2.Score.Value = 50;
      //   p3.Score.Value = 50;
      //   p4.Score.Value = 50;
      //   p5.Score.Value = 50;

      //   graph.AddNode(1, p1);
      //   graph.AddNode(2, p2);
      //   graph.AddNode(3, p3);
      //   graph.AddNode(4, p4);
      //   graph.AddNode(5, p5);
      //   Assert.IsTrue(graph.NodeCount == 5);

      //   Route r1 = new Route();
      //   Route r2 = new Route();
      //   Route r3 = new Route();
      //   Route r4 = new Route();
      //   Route r5 = new Route();

      //   r1.Distance = 200;
      //   r2.Distance = 100;
      //   r3.Distance = 100;
      //   r4.Distance = 100;
      //   r5.Distance = 100;

      //   graph.AddUndirectedEdge(1, 2, r1);
      //   graph.AddUndirectedEdge(1, 5, r2);
      //   graph.AddUndirectedEdge(2, 4, r3);
      //   graph.AddUndirectedEdge(4, 3, r4);
      //   graph.AddUndirectedEdge(5, 3, r5);
      //   Assert.IsTrue(graph.EdgeCount == 10);

      //   int? p1Score = graph[1].Score.Value;
      //   Assert.IsTrue(p1Score == 50);

      //   var p1Edges = graph.GetEdges(1);
      //   Assert.IsTrue(p1Edges.Count() == 2);

      //   var p1p2Edge = graph.GetEdge(1, 2);
      //   Assert.IsTrue(p1p2Edge.Distance == 200);
      //}
   }
}
