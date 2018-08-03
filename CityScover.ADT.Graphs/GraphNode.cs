//
// CityScover
// Version 1.0
//
// Authors: Riccardo Mariotti
// File update: 30/07/2018
//

using System.Collections.Generic;

namespace CityScover.ADT.Graphs
{
   public abstract partial class Graph<TNodeKey, TNodeData, TEdgeWeight>
      where TNodeKey : struct
      where TEdgeWeight : IGraphEdgeWeight
   {
      protected class GraphNode
      {
         private readonly TNodeKey _key;
         private readonly TNodeData _data;
         private readonly List<GraphEdge> _edges;

         #region Constructors
         private GraphNode()
         {
         }

         public GraphNode(TNodeKey key, TNodeData data)
         {
            _key = key;
            _data = data;
            _edges = new List<GraphEdge>();
         }
         #endregion

         #region Public properties
         public TNodeKey Key => _key;

         public TNodeData Data => _data;

         public IEnumerable<GraphEdge> Edges => _edges;

         public int Grade => _edges.Count;
         #endregion

         #region Public methods
         public void AddEdge(GraphNode node)
         {
            _edges.Add(new GraphEdge(this, node));
         }

         public void AddEdge(GraphNode node, TEdgeWeight weight)
         {
            _edges.Add(new GraphEdge(this, node, weight));
         }
         #endregion
      }
   }
}
