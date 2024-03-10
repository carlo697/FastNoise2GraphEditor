using UnityEditor.Experimental.GraphView;

[NodeName("Output")]
public class OutputNode : FastNoiseNode {
  public override string nodeMetadataName => "";

  public override Capabilities capabilities =>
    Capabilities.Selectable | Capabilities.Movable | Capabilities.Snappable;

  public override FastNoiseInput[] inputs => new FastNoiseInput[] {
    new FastNoiseOutputInput("Output"),
  };
}
