namespace FastNoiseGraph {
  [NodeName("Subtract", "Blends/Subtract")]
  public class SubtractNode : FastNoiseNode {
    public override string nodeMetadataName => "subtract";

    public float LHS;
    public float RHS;

    public override FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[] {
      new FastNoiseHybridInput("LHS", "LHS"),
      new FastNoiseHybridInput("RHS", "RHS")
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("LHS", LHS);
      node.Set("RHS", RHS);
    }
  }
}