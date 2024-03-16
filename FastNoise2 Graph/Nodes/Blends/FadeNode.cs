namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Fade", "Blends/Fade")]
  public class FadeNode : NoiseNode {
    public override string metadataName => "fade";

    public float fade = 0.5f;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("A", true),
      new NoiseNodeInput("B", true),
      new NoiseHybridInput("Fade", "fade"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("fade", fade);
    }
  }
}