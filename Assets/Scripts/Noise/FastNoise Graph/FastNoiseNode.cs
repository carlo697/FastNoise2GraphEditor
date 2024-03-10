using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;

public abstract class FastNoiseNode : ScriptableObject {
  public string guid;
  public Vector2 nodePosition;
  public List<FastNoiseEdge> edges = new();

  public abstract string nodeName { get; }
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
}
