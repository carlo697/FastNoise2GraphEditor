namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Domain Scale", "Modifiers/Domain Scale")]
  public class DomainScaleNode : NoiseNode {
    public override string metadataName => "domainscale";

    public float scale = 1f;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("Source", true),
      new NoiseSimpleInput("Scale", "scale"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("scale", scale);
    }
  }
}