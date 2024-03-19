namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Seed Offset", "Modifiers/Seed Offset")]
  public class SeedOffsetNode : NoiseNode {
    public override string metadataName => "seedoffset";

    public int seedOffset = 1;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("Source", true),
      new NoiseSimpleInput("Seed Offset", "seedOffset"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("seedoffset", seedOffset);
    }
  }
}