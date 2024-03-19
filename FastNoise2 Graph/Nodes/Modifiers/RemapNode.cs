namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Remap", "Modifiers/Remap")]
  public class RemapNode : NoiseNode {
    public override string metadataName => "remap";

    public float fromMin = -1f;
    public float fromMax = 1f;
    public float toMin = 0f;
    public float toMax = 1f;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("Source", true),
      new NoiseSimpleInput("From Min", "fromMin"),
      new NoiseSimpleInput("From Max", "fromMax"),
      new NoiseSimpleInput("To Min", "toMin"),
      new NoiseSimpleInput("To Max", "toMax"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("frommin", fromMin);
      node.Set("frommax", fromMax);
      node.Set("tomin", toMin);
      node.Set("tomax", toMax);
    }
  }
}