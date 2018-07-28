//
// CityScover
// Version 1.0
//
// Authors: Riccardo Mariotti
// File update: 28/07/2018
//

using System;

namespace CityScover.ADT.Graphs
{
   public abstract partial class Graph<TNodeKey, TNodeData, TEdgeWeight>
      where TNodeKey : struct
      where TEdgeWeight : IGraphEdgeWeight
   {
      protected class GraphEdge
      {
         private readonly GraphNode _sourceNode;
         private readonly GraphNode _destNode;
         private readonly TEdgeWeight _weight;
         private readonly bool _isWeighed;

         #region Constructors
         private GraphEdge()
         {
         }

         public GraphEdge(GraphNode sourceNode, GraphNode destNode)
         {
            _sourceNode = sourceNode ?? throw new ArgumentNullException("sourceNode");
            _destNode = destNode ?? throw new ArgumentNullException("destNode");
         }

         public GraphEdge(GraphNode sourceNode, GraphNode destNode, TEdgeWeight weight)
            : this(sourceNode, destNode)
         {
            //_sourceNode = sourceNode ?? throw new ArgumentNullException("sourceNode");
            //_destNode = destNode ?? throw new ArgumentNullException("destNode");
            _weight = weight;
            _isWeighed = true;
         }
         #endregion

         #region Public properties
         public GraphNode SourceNode => _sourceNode;

         public GraphNode DestNode => _destNode;

         public TEdgeWeight Weight => _weight;

         public bool IsWeighed => _isWeighed;
         #endregion
      }
   }
}
