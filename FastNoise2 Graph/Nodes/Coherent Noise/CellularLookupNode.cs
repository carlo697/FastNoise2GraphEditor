using UnityEngine;

namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Cellular Lookup", "Coherent Noise/Cellular Lookup")]
  public class CellularLookupNode : NoiseNode {
    public override string metadataName => "cellularlookup";
    public override int nodeWidth => 250;

    public float jitterModifier = 1f;
    public DistanceFunction distanceFunction = DistanceFunction.EuclideanSquared;
    [Min(0)]
    public float lookupFrequency = 0.1f;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseNodeInput("Lookup", true),
      new NoiseHybridInput("Jitter Modifier", "jitterModifier"),
      new NoiseSimpleInput("Distance Function", "distanceFunction"),
      new NoiseSimpleInput("Lookup Frequency", "lookupFrequency"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("jitterModifier", jitterModifier);
      node.Set("distanceFunction", distanceFunction.ToString());
      node.Set("lookupFrequency", lookupFrequency);
    }
  }
}