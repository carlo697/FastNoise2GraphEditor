using UnityEngine;

namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Cellular Distance", "Coherent Noise/Cellular Distance")]
  public class CellularDistanceNode : NoiseNode {
    public override string metadataName => "cellulardistance";
    public override int nodeWidth => 250;

    public float jitterModifier = 1f;
    public DistanceFunction distanceFunction = DistanceFunction.EuclideanSquared;
    [Min(0)]
    public int distanceIndex0 = 0;
    [Min(0)]
    public int distanceIndex1 = 1;
    public ReturnType returnType = ReturnType.Index0;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseHybridInput("Jitter Modifier", "jitterModifier"),
      new NoiseSimpleInput("Distance Function", "distanceFunction"),
      new NoiseSimpleInput("Distance Index 0", "distanceIndex0"),
      new NoiseSimpleInput("Distance Index 1", "distanceIndex1"),
      new NoiseSimpleInput("Return Type", "returnType"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("jitterModifier", jitterModifier);
      node.Set("distanceFunction", distanceFunction.ToString());
      node.Set("distanceIndex0", distanceIndex0);
      node.Set("distanceIndex1", distanceIndex1);
      node.Set("returnType", returnType.ToString());
    }
  }
}