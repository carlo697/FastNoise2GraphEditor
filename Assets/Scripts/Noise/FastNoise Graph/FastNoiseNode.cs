using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using System.Collections.Generic;
using System.Reflection;

public abstract class FastNoiseNode : ScriptableObject {
  public string guid;
  public Vector2 nodePosition;
  public List<FastNoiseEdge> edges = new();

  public abstract string nodeMetadataName { get; }
  public virtual int nodeWidth => 180;
  public virtual Color headerBackgroundColor => Color.black;

  public virtual Capabilities capabilities =>
    Capabilities.Selectable
    | Capabilities.Movable
    | Capabilities.Deletable
    | Capabilities.Snappable
    | Capabilities.Collapsible;

  public abstract FastNoiseInput[] inputs { get; }

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
