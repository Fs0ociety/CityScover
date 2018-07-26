using System;
using System.Collections.Generic;
using System.Linq;

namespace CityScover.ADT.Graphs
{
   public interface IGraphEdgeWeight
   {
      Func<double> Weight { get; }
   }


   public abstract class Graph<TNodeKey, TNodeData, TEdgeWeight>
      where TNodeKey : struct
      where TEdgeWeight : IGraphEdgeWeight
   {
      private readonly Dictionary<TNodeKey, GraphNode> _nodes;

      #region Constructors
      protected Graph()
      {
         _nodes = new Dictionary<TNodeKey, GraphNode>();
      }
      #endregion

      #region Public properties
      /// <summary>
      /// Restituisce il numero di nodi.
      /// </summary>
      public int NodeCount => _nodes.Count;

      /// <summary>
      /// Restituisce il numero di archi.
      /// </summary>
      public int EdgeCount => _nodes.Sum(x => x.Value.Edges.Count());

      /// <summary>
      /// Restituisce l'insieme dei nodi.
      /// </summary>
      public IEnumerable<TNodeData> Nodes => _nodes.Values.Select(x => x.Data);

      /// <summary>
      /// Restituisce il dato del nodo specificato dalla chiave.
      /// </summary>
      /// <param name="key">Chiave del nodo.</param>
      /// <returns>Dato del nodo specificato.</returns>
      public TNodeData this[TNodeKey key] => _nodes[key].Data;
      #endregion

      #region Public methods
      /// <summary>
      /// Aggiunge un nuovo nodo identificato da una chiave e da un dato.
      /// </summary>
      /// <param name="key">Chiave.</param>
      /// <param name="nodeData">Dato.</param>
      /// <exception cref="InvalidOperationException"/>
      public void AddNode(TNodeKey key, TNodeData nodeData)
      {
         // Non è possibile inserire un nuovo nodo con la stessa chiave
         if (_nodes.ContainsKey(key))
         {
            throw new InvalidOperationException();
         }
         _nodes.Add(key, new GraphNode(key, nodeData));
      }

      /// <summary>
      /// Aggiunge un nuovo arco orientato non pesato tra due nodi specificati.
      /// </summary>
      /// <param name="startNodeKey"></param>
      /// <param name="endNodeKey"></param>
      public void AddEdge(TNodeKey startNodeKey, TNodeKey endNodeKey)
      {
         // Non è possibile inserire un arco tra due nodi non esistenti
         // oppure un arco con lo stesso nodo destinazione già esistente.
         if (!_nodes.ContainsKey(startNodeKey) ||
            !_nodes.ContainsKey(endNodeKey) ||
            _nodes[startNodeKey].Edges.Where(x => Equals(x.DestNode.Key, endNodeKey)).Any())
         {
            throw new InvalidOperationException();
         }
         _nodes[startNodeKey].AddEdge(_nodes[endNodeKey]);
      }

      /// <summary>
      /// Aggiunge un nuovo arco orientato e pesato tra due nodi specificati.
      /// </summary>
      /// <param name="startNodeKey">Chiave del nodo di partenza.</param>
      /// <param name="endNodeKey">Chiave del nodo di arrivo.</param>
      /// <param name="edgeWeight">Peso attribuito.</param>
      /// <exception cref="InvalidOperationException"/>
      public void AddEdge(TNodeKey startNodeKey, TNodeKey endNodeKey, TEdgeWeight edgeWeight)
      {
         // Non è possibile inserire un arco tra due nodi non esistenti
         // oppure un arco con lo stesso nodo destinazione già esistente.
         if (!_nodes.ContainsKey(startNodeKey) ||
            !_nodes.ContainsKey(endNodeKey) ||
            _nodes[startNodeKey].Edges.Where(x => Equals(x.DestNode.Key, endNodeKey)).Any())
         {
            throw new InvalidOperationException();
         }
         _nodes[startNodeKey].AddEdge(_nodes[endNodeKey], edgeWeight);
      }

      /// <summary>
      /// Aggiunge un nuovo arco non orientato non pesato tra due nodi specificati.
      /// </summary>
      /// <param name="nodeKey1">Chiave del primo nodo.</param>
      /// <param name="nodeKey2">Chiave del secondo nodo.</param>
      /// <exception cref="InvalidOperationException"/>
      public void AddUndirectEdge(TNodeKey nodeKey1, TNodeKey nodeKey2)
      {
         if (Equals(nodeKey1, nodeKey2))
         {
            throw new InvalidOperationException();
         }
         AddEdge(nodeKey1, nodeKey2);
         AddEdge(nodeKey2, nodeKey1);
      }

      /// <summary>
      /// Aggiunge un nuovo arco non orientato e pesato tra due nodi specificati.
      /// </summary>
      /// <param name="nodeKey1">Chiave del primo nodo.</param>
      /// <param name="nodeKey2">Chiave del secondo nodo.</param>
      /// <param name="edgeWeight">Peso attribuito.</param>
      /// <exception cref="InvalidOperationException"/>
      public void AddUndirectEdge(TNodeKey nodeKey1, TNodeKey nodeKey2, TEdgeWeight edgeWeight)
      {
         if (Equals(nodeKey1, nodeKey2))
         {
            throw new InvalidOperationException();
         }
         AddEdge(nodeKey1, nodeKey2, edgeWeight);
         AddEdge(nodeKey2, nodeKey1, edgeWeight);
      }

      /// <summary>
      /// Determina se il nodo specificato esiste nel grafo.
      /// </summary>
      /// <param name="nodeKey">Chiave del nodo.</param>
      /// <returns>
      /// True se contiene il nodo con la chiave specificata, altrimenti false.
      ///</returns>
      public bool ContainsNode(TNodeKey nodeKey) => _nodes.ContainsKey(nodeKey);

      /// <summary>
      /// Ritorna le chiavi dei nodi predecessori del nodo specificato.
      /// </summary>
      /// <param name="nodeKey">Chiave del nodo.</param>
      /// <returns>Insieme delle chiavi dei nodi predecessori.</returns>
      /// <exception cref="InvalidOperationException"</exception>
      public IEnumerable<TNodeKey> GetPredecessorNodes(TNodeKey nodeKey)
      {
         if (!_nodes.ContainsKey(nodeKey))
         {
            throw new InvalidOperationException();
         }

         return _nodes
            .SelectMany(x => x.Value.Edges.Where(y => Equals(y.DestNode.Key, nodeKey)))
            .Distinct()
            .Select(x => x.SourceNode.Key);
      }

      /// <summary>
      /// Ritorna le chiavi dei nodi successori del nodo specificato.
      /// </summary>
      /// <param name="nodeKey">Chiave del nodo.</param>
      /// <returns>Insieme delle chiavi dei nodi successori.</returns>
      /// <exception cref="InvalidOperationException"</exception>
      public IEnumerable<TNodeKey> GetSuccessorNodes(TNodeKey nodeKey)
      {
         if (!_nodes.ContainsKey(nodeKey))
         {
            throw new InvalidOperationException();
         }

         return _nodes[nodeKey].Edges.Select(x => x.DestNode.Key);
      }
      #endregion

      #region Classes

      #region GraphNode
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
      #endregion

      #region GraphEdge
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
      #endregion

      #endregion
   }
}
