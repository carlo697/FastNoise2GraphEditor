namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Multiply", "Blends/Multiply")]
  public class MultiplyNode : FastNoiseNode {
    public override string metadataName => "multiply";

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