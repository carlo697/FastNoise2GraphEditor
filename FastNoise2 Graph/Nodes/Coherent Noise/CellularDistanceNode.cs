using UnityEngine;

namespace FastNoiseGraph {
  [System.Serializable]
  [NodeName("Cellular Distance", "Coherent Noise/Cellular Distance")]
  public class CellularDistanceNode : FastNoiseNode {
    public override string metadataName => "cellulardistance";
    public override int nodeWidth => 250;

    public float jitterModifier = 1f;
    public DistanceFunction distanceFunction = DistanceFunction.EuclideanSquared;
    [Min(0)]
    public int distanceIndex0 = 0;
    [Min(0)]
    public int distanceIndex1 = 1;
    public ReturnType returnType = ReturnType.Index0;

    public override FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[] {
      new FastNoiseHybridInput("Jitter Modifier", "jitterModifier"),
      new FastNoiseSimpleInput("Distance Function", "distanceFunction"),
      new FastNoiseSimpleInput("Distance Index 0", "distanceIndex0"),
      new FastNoiseSimpleInput("Distance Index 1", "distanceIndex1"),
      new FastNoiseSimpleInput("Return Type", "returnType"),
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