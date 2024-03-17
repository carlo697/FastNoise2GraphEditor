using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastNoise2Graph {
  [System.Serializable]
  public abstract class NoiseNode {
    public string guid;
    public Vector2 nodePosition;
    public List<NoiseEdge> edges = new();

    public abstract string metadataName { get; }
    public virtual int nodeWidth => 200;
    public virtual Color headerBackgroundColor => Color.black;

    public virtual Capabilities capabilities =>
      Capabilities.Selectable
      | Capabilities.Movable
      | Capabilities.Deletable
      | Capabilities.Snappable
      | Capabilities.Collapsible
      | Capabilities.Copiable;

    public virtual NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[0];

    public NoiseNode() {
      guid = Guid.NewGuid().ToString();
    }

    public virtual void ApplyValues(FastNoise node) { }

    public static string GetNodeName(Type type) {
      NodeNameAttribute nameAttribute = type.GetCustomAttribute<NodeNameAttribute>();
      return nameAttribute?.name ?? type.Name;
    }

    public static string GetNodeMenuName(Type type) {
      NodeNameAttribute nameAttribute = type.GetCustomAttribute<NodeNameAttribute>();
      return nameAttribute?.menuName ?? type.Name;
    }

    public static string GetNodeName(NoiseNode node) {
      return GetNodeName(node.GetType());
    }
  }
}