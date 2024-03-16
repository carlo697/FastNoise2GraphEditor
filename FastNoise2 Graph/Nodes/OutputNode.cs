using UnityEditor.Experimental.GraphView;

namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Output")]
  public class OutputNode : NoiseNode {
    public override string metadataName => "";

    public override Capabilities capabilities =>
      Capabilities.Selectable | Capabilities.Movable | Capabilities.Snappable;

    public override NoiseInput[] inputs => m_inputs;
    private NoiseInput[] m_inputs = new NoiseInput[] {
      new NoiseOutputInput("Output"),
    };
  }
}