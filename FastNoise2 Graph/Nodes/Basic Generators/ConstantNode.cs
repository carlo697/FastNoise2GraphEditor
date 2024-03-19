namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Constant", "Basic Generators/Constant")]
  public class ConstantNode : NoiseNode {
    public override string metadataName => "constant";

    public float value = 1f;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseSimpleInput("Value", "value"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("value", value);
    }
  }
}