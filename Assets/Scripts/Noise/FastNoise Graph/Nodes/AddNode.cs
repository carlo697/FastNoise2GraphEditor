public class AddNode : FastNoiseNode {
  public override string nodeName => "Add";
  public override string nodeMetadataName => "add";

  public float RHS;

  public override FastNoiseInput[] inputs => new FastNoiseInput[] {
    new FastNoiseNodeInput("LHS", true),
    new FastNoiseHybridInput("RHS", "RHS")
  };

  public override void ApplyValues(FastNoise node) {
    node.Set("RHS", RHS);
  }
}
