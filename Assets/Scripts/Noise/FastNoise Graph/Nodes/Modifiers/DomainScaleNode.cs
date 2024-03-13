using UnityEngine;

namespace FastNoiseGraph {
  [NodeName("Domain Scale", "Modifiers/Domain Scale")]
  public class DomainScaleNode : FastNoiseNode {
    public override string metadataName => "domainscale";

    public float scale = 1f;

    public override FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[] {
      new FastNoiseNodeInput("Source", true),
      new FastNoiseSimpleInput("Scale", "scale"),
    };

    public override void ApplyValues(FastNoise node) {
      node.Set("scale", scale);
    }
  }
}