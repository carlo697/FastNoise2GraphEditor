namespace FastNoiseGraph {
  [System.Serializable]
  [NodeName("Add", "Blends/Add")]
  public class AddNode : FastNoiseNode {
    public override string metadataName => "add";

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