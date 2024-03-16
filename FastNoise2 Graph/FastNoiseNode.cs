using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastNoise2Graph {
  [System.Serializable]
  public abstract class FastNoiseNode {
    public string guid;
    public Vector2 nodePosition;
    public List<FastNoiseEdge> edges = new();

    public abstract string metadataName { get; }
    public virtual int nodeWidth => 200;
    public virtual Color headerBackgroundColor => Color.black;

    public virtual Capabilities capabilities =>
      Capabilities.Selectable
      | Capabilities.Movable
      | Capabilities.Deletable
      | Capabilities.Snappable
      | Capabilities.Collapsible;

    public virtual FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[0];

    public FastNoiseNode() {
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

    public static string GetNodeName(FastNoiseNode node) {
      return GetNodeName(node.GetType());
    }
  }
}