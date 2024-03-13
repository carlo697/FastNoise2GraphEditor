namespace FastNoiseGraph {
  [NodeName("Fade", "Blends/Fade")]
  public class FadeNode : FastNoiseNode {
    public override string metadataName => "fade";

    public float fade = 0.5f;

    public override FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[] {
      new FastNoiseNodeInput("A", true),
      new FastNoiseNodeInput("B", true),
      new FastNoiseHybridInput("Fade", "fade"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("fade", fade);
    }
  }
}