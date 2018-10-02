//
// CityScover
// Version 1.0
//
// Authors: Riccardo Mariotti
// File update: 16/09/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.ADT.Graphs
{
   public abstract partial class Graph<TNodeKey, TNodeData, TEdgeWeight>
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
      public void AddUndirectedEdge(TNodeKey nodeKey1, TNodeKey nodeKey2)
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
      public void AddUndirectedEdge(TNodeKey nodeKey1, TNodeKey nodeKey2, TEdgeWeight edgeWeight)
      {
         if (Equals(nodeKey1, nodeKey2))
         {
            throw new InvalidOperationException();
         }
         AddEdge(nodeKey1, nodeKey2, edgeWeight);
         AddEdge(nodeKey2, nodeKey1, edgeWeight);
      }

      /// <summary>
      /// Rimuove nel grafo il nodo identificato dalla chiave passata come parametro.
      /// Rimuovendo il nodo automaticamente si rimuovono tutti gli archi incidenti su tale nodo.
      /// </summary>
      /// <param name="key">Chiave del nodo.</param>
      /// <exception cref="InvalidOperationException"/>
      public void RemoveNode(TNodeKey key)
      {
         // Non è possibile eliminare un nodo che non esiste nel grafo.
         if (!(_nodes.Remove(key)))
         {
            throw new InvalidOperationException();
         }
      }

      /// <summary>
      /// Rimuove nel grafo l'arco orientato identificato dalle chiavi passate come parametri.
      /// </summary>
      /// <param name="startNodeKey">Chiave del nodo sorgente.</param>
      /// <param name="endNodeKey">Chiave del nodo destinazione.</param>
      /// <exception cref="InvalidOperationException"/>
      public void RemoveEdge(TNodeKey startNodeKey, TNodeKey endNodeKey)
      {
         var edge = _nodes.SelectMany(x => x.Value.Edges
                          .Where(y => Equals(y.SourceNode.Key, startNodeKey))
                          .Where(z => Equals(z.DestNode.Key, endNodeKey))).FirstOrDefault();

         // Se l'arco non è stato trovato non posso rimuovere nulla.
         if (edge == null)
         {
            throw new InvalidOperationException();
         }

         _nodes[startNodeKey].RemoveEdge(edge);
      }

      /// <summary>
      /// Rimuove nel grafo l'arco non orientato identificato dalle chiavi passate come parametri.
      /// In pratica rimuove la coppia di archi orientati (nodeKey1, nodeKey2) e (nodeKey2, nodeKey1).
      /// </summary>
      /// <param name="nodeKey1">Chiave del nodo sorgente.</param>
      /// <param name="nodeKey2">Chiave del nodo destinazione.</param>
      /// <exception cref="InvalidOperationException"/>
      public void RemoveUndirectedEdge(TNodeKey nodeKey1, TNodeKey nodeKey2)
      {
         // Non posso rimuovere un'arco degenere (entrambi i nodi sono uguali)
         // perchè non consentito.
         if (Equals(nodeKey1, nodeKey2))
         {
            throw new InvalidOperationException();
         }

         RemoveEdge(nodeKey1, nodeKey2);
         RemoveEdge(nodeKey2, nodeKey1);
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
      /// Restituisce l'insieme dei dati degli archi specificata la chiave del nodo.
      /// </summary>
      /// <param name="nodeKey">Chiave del nodo.</param>
      /// <returns></returns>
      public IEnumerable<TEdgeWeight> GetEdges(TNodeKey nodeKey)
      {
         if (!_nodes.ContainsKey(nodeKey))
         {
            throw new InvalidOperationException();
         }

         //return _nodes.SelectMany(x => x.Value.Edges.Select(k => k.Weight));
         return _nodes[nodeKey].Edges.Select(x => x.Weight);
      }

      /// <summary>
      /// Restituisce il dato dell'arco specificato dalla chiave.
      /// </summary>
      /// <param name="startNodeKey">Chiave del primo nodo.</param>
      /// <param name="endNodeKey">Chiave del secondo nodo.</param>
      /// <returns></returns>
      public TEdgeWeight GetEdge(TNodeKey startNodeKey, TNodeKey endNodeKey)
      {
         if (!_nodes.ContainsKey(startNodeKey) || !_nodes.ContainsKey(endNodeKey))
         {
            throw new InvalidOperationException();
         }

         return _nodes
            .SelectMany(x => x.Value.Edges
            .Where(y => Equals(y.SourceNode.Key, startNodeKey)))
            .Where(z => Equals(z.DestNode.Key, endNodeKey))
            .Select(k => k.Weight).FirstOrDefault();
      }

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

      /// <summary>
      /// Ritorna l'insieme dei nodi adiacenti al nodo specificato come parametro.
      /// </summary>
      /// <param name="nodeKey"></param>
      /// <returns></returns>
      public IEnumerable<TNodeKey> GetAdjacentNodes(TNodeKey nodeKey)
      {
         if (!_nodes.ContainsKey(nodeKey))
         {
            throw new InvalidOperationException();
         }

         return _nodes[nodeKey]
            .Edges
            .Where((edge) => !edge.DestNode.Key.Equals(nodeKey))
            .Select(x => x.DestNode.Key);
      }

      /// <summary>
      /// Ritorna il grado di un nodo, corrispondente al numero di archi incidenti su esso.
      /// </summary>
      /// <param name="nodeKey"></param>
      /// <returns></returns>
      public int GetNodeGrade(TNodeKey nodeKey)
      {
         if (!ContainsNode(nodeKey))
         {
            throw new InvalidOperationException();
         }
         return _nodes[nodeKey].Grade;
      }

      public bool AreAdjacentEdges(TNodeKey startNodeKey1, TNodeKey endNodeKey1, TNodeKey startNodeKey2, TNodeKey endNodeKey2)
      {
         if (!_nodes.ContainsKey(startNodeKey1) || 
             !_nodes.ContainsKey(endNodeKey1) || 
             !_nodes.ContainsKey(startNodeKey2) || 
             !_nodes.ContainsKey(endNodeKey2) ||
             startNodeKey1.Equals(endNodeKey1) ||
             startNodeKey2.Equals(endNodeKey2) ||
             (startNodeKey1.Equals(startNodeKey2) && endNodeKey1.Equals(endNodeKey2)))
         {
            throw new InvalidOperationException();
         }

         var firstNodeEdges = _nodes[startNodeKey1].Edges;
         var secondNodeEdges = _nodes[startNodeKey2].Edges;

         var result = firstNodeEdges
            .Where(edge => secondNodeEdges
            .Any(edge2 => edge2.SourceNode.Key.Equals(edge.DestNode.Key)
                          || edge2.DestNode.Key.Equals(edge.SourceNode.Key)
                          && edge2.DestNode.Key.Equals(endNodeKey2)) &&
            edge.SourceNode.Key.Equals(startNodeKey1));

         return result.Count() == 0 ? false : true;
      }
      #endregion      
   }
}
