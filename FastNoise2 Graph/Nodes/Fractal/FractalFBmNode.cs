using UnityEngine;

namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Fractal FBm", "Fractal/Fractal FBm")]
  public class FractalFBmNode : NoiseNode {
    public override string metadataName => "fractalfbm";
    public override int nodeWidth => 250;

    public float gain = 0.5f;
    public float weightedStrength = 0f;
    [Min(1)]
    public int octaves = 3;
    public float lacunarity = 2f;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("Source", true),
      new NoiseHybridInput("Gain", "gain"),
      new NoiseHybridInput("Weighted Strength", "weightedStrength"),
      new NoiseSimpleInput("Octaves", "octaves"),
      new NoiseSimpleInput("Lacunarity", "lacunarity"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("gain", gain);
      node.Set("weightedStrength", weightedStrength);
      node.Set("octaves", octaves);
      node.Set("lacunarity", lacunarity);
    }
  }
}