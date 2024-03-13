namespace FastNoiseGraph {
  [NodeName("Max", "Blends/Max")]
  public class MaxNode : FastNoiseNode {
    public override string nodeMetadataName => "max";

    public float RHS;

    public override FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[] {
      new FastNoiseNodeInput("LHS", true),
      new FastNoiseHybridInput("RHS", "RHS")
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("RHS", RHS);
    }
  }
}