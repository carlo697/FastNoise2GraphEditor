namespace FastNoiseGraph {
  [NodeName("Min", "Blends/Min")]
  public class MinNode : FastNoiseNode {
    public override string metadataName => "min";

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