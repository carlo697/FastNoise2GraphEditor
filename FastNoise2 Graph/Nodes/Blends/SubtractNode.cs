namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Subtract", "Blends/Subtract")]
  public class SubtractNode : NoiseNode {
    public override string metadataName => "subtract";

    public float LHS;
    public float RHS;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseHybridInput("LHS", "LHS"),
      new NoiseHybridInput("RHS", "RHS")
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("LHS", LHS);
      node.Set("RHS", RHS);
    }
  }
}