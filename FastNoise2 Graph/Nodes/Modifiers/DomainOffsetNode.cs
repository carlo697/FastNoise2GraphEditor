namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Domain Offset", "Modifiers/Domain Offset")]
  public class DomainOffsetNode : NoiseNode {
    public override string metadataName => "domainoffset";

    public float offsetX;
    public float offsetY;
    public float offsetZ;
    public float offsetW;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("Source", true),
      new NoiseHybridInput("Offset X", "offsetX"),
      new NoiseHybridInput("Offset Y", "offsetY"),
      new NoiseHybridInput("Offset Z", "offsetZ"),
      new NoiseHybridInput("Offset W", "offsetW"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("offsetx", offsetX);
      node.Set("offsety", offsetY);
      node.Set("offsetz", offsetZ);
      node.Set("offsetw", offsetW);
    }
  }
}