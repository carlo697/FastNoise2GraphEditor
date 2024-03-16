namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Add", "Blends/Add")]
  public class AddNode : NoiseNode {
    public override string metadataName => "add";

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