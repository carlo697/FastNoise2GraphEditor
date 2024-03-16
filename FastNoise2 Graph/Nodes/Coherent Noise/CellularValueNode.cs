using UnityEngine;

namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Cellular Value", "Coherent Noise/Cellular Value")]
  public class CellularValueNode : NoiseNode {
    public override string metadataName => "cellularvalue";
    public override int nodeWidth => 250;

    public float jitterModifier = 1f;
    public DistanceFunction distanceFunction = DistanceFunction.EuclideanSquared;
    [Min(0)]
    public int valueIndex = 0;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseHybridInput("Jitter Modifier", "jitterModifier"),
      new NoiseSimpleInput("Distance Function", "distanceFunction"),
      new NoiseSimpleInput("Value Index", "valueIndex"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("jitterModifier", jitterModifier);
      node.Set("distanceFunction", distanceFunction.ToString());
      node.Set("valueIndex", valueIndex);
    }
  }
}