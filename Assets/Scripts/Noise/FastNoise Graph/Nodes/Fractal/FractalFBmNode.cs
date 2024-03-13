using UnityEngine;

namespace FastNoiseGraph {
  [NodeName("Fractal FBm", "Fractal/Fractal FBm")]
  public class FractalFBmNode : FastNoiseNode {
    public override string metadataName => "fractalfbm";
    public override int nodeWidth => 250;

    public float gain = 0.5f;
    public float weightedStrength = 0f;
    [Min(1)]
    public int octaves = 3;
    public float lacunarity = 2f;

    public override FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[] {
      new FastNoiseNodeInput("Source", true),
      new FastNoiseHybridInput("Gain", "gain"),
      new FastNoiseHybridInput("Weighted Strength", "weightedStrength"),
      new FastNoiseSimpleInput("Octaves", "octaves"),
      new FastNoiseSimpleInput("Lacunarity", "lacunarity"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("gain", gain);
      node.Set("weightedStrength", weightedStrength);
      node.Set("octaves", octaves);
      node.Set("lacunarity", lacunarity);
    }
  }
}