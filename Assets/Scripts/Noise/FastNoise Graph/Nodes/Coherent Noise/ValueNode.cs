[NodeName("Value", "Coherent Noise/Value")]
public class ValueNode : FastNoiseNode {
  public override string nodeMetadataName => "value";

  public override FastNoiseInput[] inputs => new FastNoiseInput[0];
}