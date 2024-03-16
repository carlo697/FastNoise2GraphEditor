using UnityEngine;

namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Cellular Lookup", "Coherent Noise/Cellular Lookup")]
  public class CellularLookupNode : FastNoiseNode {
    public override string metadataName => "cellularlookup";
    public override int nodeWidth => 250;

    public float jitterModifier = 1f;
    public DistanceFunction distanceFunction = DistanceFunction.EuclideanSquared;
    [Min(0)]
    public float lookupFrequency = 0.1f;

    public override FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[] {
      new FastNoiseNodeInput("Lookup", true),
      new FastNoiseHybridInput("Jitter Modifier", "jitterModifier"),
      new FastNoiseSimpleInput("Distance Function", "distanceFunction"),
      new FastNoiseSimpleInput("Lookup Frequency", "lookupFrequency"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("jitterModifier", jitterModifier);
      node.Set("distanceFunction", distanceFunction.ToString());
      node.Set("lookupFrequency", lookupFrequency);
    }
  }
}