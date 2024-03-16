namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Divide", "Blends/Divide")]
  public class DivideNode : NoiseNode {
    public override string metadataName => "divide";

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