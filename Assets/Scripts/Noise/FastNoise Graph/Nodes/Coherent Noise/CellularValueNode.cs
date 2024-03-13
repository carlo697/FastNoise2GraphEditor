using UnityEngine;

namespace FastNoiseGraph {
  [NodeName("Cellular Value", "Coherent Noise/Cellular Value")]
  public class CellularValueNode : FastNoiseNode {
    public override string nodeMetadataName => "cellularvalue";
    public override int nodeWidth => 250;

    public float jitterModifier = 1f;
    public DistanceFunction distanceFunction = DistanceFunction.EuclideanSquared;
    [Min(0)]
    public int valueIndex = 0;

    public override FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[] {
      new FastNoiseHybridInput("Jitter Modifier", "jitterModifier"),
      new FastNoiseSimpleInput("Distance Function", "distanceFunction"),
      new FastNoiseSimpleInput("Value Index", "valueIndex"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("jitterModifier", jitterModifier);
      node.Set("distanceFunction", distanceFunction.ToString());
      node.Set("valueIndex", valueIndex);
    }
  }
}