using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace FastNoise2Graph {
  [CreateAssetMenu(menuName = "Fast Noise Tree")]
  public class NoiseTree : ScriptableObject {
    [SerializeReference]
    [HideInInspector]
    public NoiseNode outputNode;

    [SerializeReference]
    [HideInInspector]
    public List<NoiseNode> nodes = new List<NoiseNode>();
    private Dictionary<NoiseNode, FastNoise> m_nodesCache;

    public static string outputNodeName => m_outputNodeName;
    private static string m_outputNodeName = "Output";

    public NoiseTree() {
      NoiseNode node = new NoiseNode(outputNodeName);
      nodes.Add(node);
      outputNode = node;
    }

    public static bool IsOutputNode(NoiseNode node) {
      return node.metadataName == m_outputNodeName;
    }

    public NoiseNode AddNode(string name) {
      NoiseNode node = new NoiseNode(name);

      Undo.RecordObject(this, "FastNoise Tree (Add Node)");
      nodes.Add(node);

      if (IsOutputNode(node)) {
        outputNode = node;
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

    private FastNoise GetFastNoise(NoiseNode node, bool isOutput, Dictionary<NoiseNode, FastNoise> cache) {
      // Clear the cache if necessary
      if (isOutput) {
        cache.Clear();
      }

      // The node was previosly created, let's return it
      if (cache.ContainsKey(node)) {
        return cache[node];
      }

      // Check if node inputs have connections
      FastNoise.Metadata metadata = FastNoise.GetMetadata(node.metadataName);
      foreach (var (memberName, member) in metadata.members) {
        if (member.type == FastNoise.Metadata.Member.Type.NodeLookup) {
          // Find an edge for this member
          bool found = false;
          for (int edgeIndex = 0; edgeIndex < node.edges.Count; edgeIndex++) {
            NoiseEdge edge = node.edges[edgeIndex];

            if (edge.parentPortName == memberName) {
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
      cache.Add(node, instancedNode);

      // Debug.Log(node.metadataName);

      // Apply values of variables
      for (int i = 0; i < node.intValues.Count; i++) {
        NoiseIntMember memberValue = node.intValues[i];
        instancedNode.Set(memberValue.name, memberValue.value);
      }
      for (int i = 0; i < node.floatValues.Count; i++) {
        NoiseFloatMember memberValue = node.floatValues[i];
        instancedNode.Set(memberValue.name, memberValue.value);
      }
      for (int i = 0; i < node.enumValues.Count; i++) {
        NoiseStringMember memberValue = node.enumValues[i];
        instancedNode.Set(memberValue.name, memberValue.value);
      }

      // Iterate over the connections to create and connect those nodes to this one
      for (int i = 0; i < node.edges.Count; i++) {
        NoiseEdge edge = node.edges[i];

        // Create or get the node connected to the edge
        FastNoise childNode = GetFastNoise(edge.childNode, false, cache);
        if (childNode == null) {
          return null;
        }

        // Set the connection
        instancedNode.Set(edge.parentPortName, childNode);

        // Debug.Log($"{edge.parentPortName}, parent: {node.metadataName}");
      }

      return instancedNode;
    }

    public FastNoise GetFastNoise(NoiseNode node, Dictionary<NoiseNode, FastNoise> cache) {
      if (!IsOutputNode(node)) {
        return GetFastNoise(node, true, cache);
      }

      if (node.edges.Count > 0) {
        return GetFastNoise(node.edges[0].childNode, true, cache);
      }

      return null;
    }

    public FastNoise GetFastNoise(NoiseNode node) {
      if (m_nodesCache == null) {
        m_nodesCache = new();
      }

      return GetFastNoise(node, m_nodesCache);
    }

    public FastNoise GetFastNoiseSafe(NoiseNode node) {
      return GetFastNoise(node, new());
    }

    public FastNoise GetFastNoise() {
      if (m_nodesCache == null) {
        m_nodesCache = new();
      }

      return GetFastNoise(outputNode, m_nodesCache);
    }

    public FastNoise GetFastNoiseSafe() {
      return GetFastNoise(outputNode, new());
    }
  }
}