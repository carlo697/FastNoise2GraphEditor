using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using FastNoise2Graph.Nodes;

namespace FastNoise2Graph {
  [CreateAssetMenu(menuName = "Fast Noise Tree")]
  public class NoiseTree : ScriptableObject {
    [SerializeReference]
    [HideInInspector]
    public OutputNode outputNode;

    [SerializeReference]
    [HideInInspector]
    public List<NoiseNode> nodes = new List<NoiseNode>();
    private Dictionary<NoiseNode, FastNoise> m_nodesCache;

    public NoiseTree() {
      OutputNode node = new OutputNode();
      nodes.Add(node);
      outputNode = node;
    }

    public NoiseNode AddNode(Type type) {
      NoiseNode node = (NoiseNode)Activator.CreateInstance(type);

      Undo.RecordObject(this, "FastNoise Tree (Add Node)");
      nodes.Add(node);

      if (node is OutputNode _outputNode) {
        outputNode = _outputNode;
      }

      EditorUtility.SetDirty(this);

      return node;
    }

    public void DeleteNode(NoiseNode node) {
      Undo.RecordObject(this, "FastNoise Tree (Delete Node)");

      if (outputNode == node) {
        outputNode = null;
      }

      nodes.Remove(node);

      EditorUtility.SetDirty(this);
    }

    private FastNoise GetFastNoise(NoiseNode node, bool isOutput) {
      // Create or clear the cache if necessary
      if (isOutput) {
        if (m_nodesCache != null) {
          m_nodesCache.Clear();
        } else {
          m_nodesCache = new Dictionary<NoiseNode, FastNoise>();
        }
      }

      // The node was previosly created, let's return it
      if (m_nodesCache.ContainsKey(node)) {
        return m_nodesCache[node];
      }

      // Check if the mandatory inputs have connections
      for (int inputIndex = 0; inputIndex < node.inputs.Length; inputIndex++) {
        NoiseInput input = node.inputs[inputIndex];

        if (input.acceptsEdge && input.isEdgeMandatory) {
          // Find an edge for this input
          bool found = false;
          for (int edgeIndex = 0; edgeIndex < node.edges.Count; edgeIndex++) {
            NoiseEdge edge = node.edges[edgeIndex];

            if (edge.parentPortIndex == inputIndex) {
              found = true;
              break;
            }
          }

          if (!found) {
            return null;
          }
        }
      }

      // Create the new node
      FastNoise instancedNode = new FastNoise(node.metadataName);
      m_nodesCache.Add(node, instancedNode);

      // Debug.Log(node.nodeMetadataName);

      // Apply values of variables
      node.ApplyValues(instancedNode);
      // Type nodeType = node.GetType();
      // for (int i = 0; i < node.inputs.Length; i++) {
      //   FastNoiseInput input = node.inputs[i];

      //   if (input.fieldPath != null) {
      //     FieldInfo field = nodeType.GetField(input.fieldPath);

      //     object value = field.GetValue(node);
      //     if (value is float floatValue) {
      //       instancedNode.Set(input.label, floatValue);
      //     } else if (value is int intValue) {
      //       instancedNode.Set(input.label, intValue);
      //     } else if (value is Enum enumValue) {
      //       instancedNode.Set(input.label, enumValue.ToString());
      //     }
      //   }
      // }

      // Iterate over the connections to create and connect those nodes to this one
      for (int i = 0; i < node.edges.Count; i++) {
        NoiseEdge edge = node.edges[i];
        NoiseInput port = node.inputs[edge.parentPortIndex];

        // Create or get the node
        FastNoise childNode = GetFastNoise(edge.childNode, false);
        if (childNode == null) {
          return null;
        }

        // Set the connection
        string memberName = port.label;
        instancedNode.Set(memberName, childNode);

        // Debug.Log($"{memberName}, parent: {node.nodeMetadataName}");
      }

      return instancedNode;
    }

    public FastNoise GetFastNoise(NoiseNode node) {
      if (node is not OutputNode) {
        return GetFastNoise(node, true);
      }

      if (node.edges.Count > 0) {
        return GetFastNoise(node.edges[0].childNode, true);
      }

      return null;
    }

    public FastNoise GetFastNoise() {
      return GetFastNoise(outputNode);
    }
  }
}