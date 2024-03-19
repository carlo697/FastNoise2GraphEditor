namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Domain Rotate", "Modifiers/Domain Rotate")]
  public class DomainRotateNode : NoiseNode {
    public override string metadataName => "domainrotate";

    public float yaw;
    public float pitch;
    public float roll;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("Source", true),
      new NoiseSimpleInput("Yaw", "yaw"),
      new NoiseSimpleInput("Pitch", "pitch"),
      new NoiseSimpleInput("Roll", "roll"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("yaw", yaw);
      node.Set("pitch", pitch);
      node.Set("roll", roll);
    }
  }
}