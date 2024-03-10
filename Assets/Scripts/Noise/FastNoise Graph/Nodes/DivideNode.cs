[NodeName("Divide", "Blends/Divide")]
public class DivideNode : FastNoiseNode {
  public override string nodeMetadataName => "divide";

  public float LHS;
  public float RHS;

  public override FastNoiseInput[] inputs => new FastNoiseInput[] {
    new FastNoiseHybridInput("LHS", "LHS"),
    new FastNoiseHybridInput("RHS", "RHS")
  };

  public override void ApplyValues(FastNoise node) {
    node.Set("LHS", LHS);
    node.Set("RHS", RHS);
  }
}
