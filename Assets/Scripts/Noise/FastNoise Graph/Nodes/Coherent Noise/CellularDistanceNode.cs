using UnityEngine;

[NodeName("Cellular Distance", "Coherent Noise/Cellular Distance")]
public class CellularDistanceNode : FastNoiseNode {
  public override string nodeMetadataName => "cellulardistance";
  public override int nodeWidth => 250;

  public float jitterModifier = 1f;
  public DistanceFunction distanceFunction = DistanceFunction.EuclideanSquared;
  [Min(0)]
  public int distanceIndex0 = 0;
  public int distanceIndex1 = 1;
  public ReturnType returnType = ReturnType.Index0;

  public override FastNoiseInput[] inputs => new FastNoiseInput[] {
    new FastNoiseHybridInput("Jitter Modifier", "jitterModifier"),
    new FastNoiseFloatInput("Distance Function", "distanceFunction"),
    new FastNoiseFloatInput("Distance Index 0", "distanceIndex0"),
    new FastNoiseFloatInput("Distance Index 1", "distanceIndex1"),
    new FastNoiseFloatInput("Return Type", "returnType"),
  };

  public override void ApplyValues(FastNoise node) {
    node.Set("jitterModifier", jitterModifier);
    node.Set("distanceFunction", distanceFunction.ToString());
    node.Set("distanceIndex0", distanceIndex0);
    node.Set("distanceIndex1", distanceIndex1);
    node.Set("returnType", returnType.ToString());
  }
}
