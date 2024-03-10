using UnityEditor.Experimental.GraphView;

public class OutputNode : FastNoiseNode {
  public override string nodeName => "Output";
  public override string nodeMetadataName => "None";

  public override Capabilities capabilities =>
    Capabilities.Selectable | Capabilities.Movable | Capabilities.Snappable;

  public override FastNoiseInput[] inputs => new FastNoiseInput[] {
    new FastNoiseOutputInput("Output"),
  };
}
