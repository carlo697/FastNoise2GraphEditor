[NodeName("Min")]
public class MinNode : FastNoiseNode {
  public override string nodeMetadataName => "min";

  public float RHS;

  public override FastNoiseInput[] inputs => new FastNoiseInput[] {
    new FastNoiseNodeInput("LHS", true),
    new FastNoiseHybridInput("RHS", "RHS")
  };

  public override void ApplyValues(FastNoise node) {
    node.Set("RHS", RHS);
  }
}
