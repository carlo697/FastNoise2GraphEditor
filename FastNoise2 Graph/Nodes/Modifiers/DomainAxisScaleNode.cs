namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Domain Axis Scale", "Modifiers/Domain Axis Scale")]
  public class DomainAxisScaleNode : NoiseNode {
    public override string metadataName => "domainaxisscale";

    public float scaleX = 1f;
    public float scaleY = 1f;
    public float scaleZ = 1f;
    public float scaleW = 1f;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("Source", true),
      new NoiseSimpleInput("X Scale", "scaleX"),
      new NoiseSimpleInput("Y Scale", "scaleY"),
      new NoiseSimpleInput("Z Scale", "scaleZ"),
      new NoiseSimpleInput("W Scale", "scaleW"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("scalex", scaleX);
      node.Set("scaley", scaleY);
      node.Set("scalez", scaleZ);
      node.Set("scalew", scaleW);
    }
  }
}