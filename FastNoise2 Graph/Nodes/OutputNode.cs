using UnityEditor.Experimental.GraphView;

namespace FastNoise2Graph.Nodes {
  [System.Serializable]
  [NodeName("Output")]
  public class OutputNode : FastNoiseNode {
    public override string metadataName => "";

    public override Capabilities capabilities =>
      Capabilities.Selectable | Capabilities.Movable | Capabilities.Snappable;

    public override FastNoiseInput[] inputs => m_inputs;
    private FastNoiseInput[] m_inputs = new FastNoiseInput[] {
      new FastNoiseOutputInput("Output"),
    };
  }
}