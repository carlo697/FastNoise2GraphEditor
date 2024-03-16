namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Max", "Blends/Max")]
  public class MaxNode : NoiseNode {
    public override string metadataName => "max";

    public float RHS;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("LHS", true),
      new NoiseHybridInput("RHS", "RHS")
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("RHS", RHS);
    }
  }
}