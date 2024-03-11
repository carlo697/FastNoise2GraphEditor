using UnityEngine;

[NodeName("Fractal FBm", "Fractal/Fractal FBm")]
public class FractalFBmNode : FastNoiseNode {
  public override string nodeMetadataName => "fractalfbm";
  public override int nodeWidth => 250;

  public float gain = 0.5f;
  public float weightedStrength = 0f;
  [Min(1)]
  public int octaves = 3;
  public float lacunarity = 2f;

  public override FastNoiseInput[] inputs => new FastNoiseInput[] {
    new FastNoiseNodeInput("Source", true),
    new FastNoiseHybridInput("Gain", "gain"),
    new FastNoiseHybridInput("Weighted Strength", "weightedStrength"),
    new FastNoiseFloatInput("Octaves", "octaves"),
    new FastNoiseFloatInput("Lacunarity", "lacunarity"),
  };

  public override void ApplyValues(FastNoise node) {
    node.Set("gain", gain);
    node.Set("weightedStrength", weightedStrength);
    node.Set("octaves", octaves);
    node.Set("lacunarity", lacunarity);
  }
}
