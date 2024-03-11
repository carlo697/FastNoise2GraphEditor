using UnityEngine;

public abstract class FastNoiseInput {
  public bool acceptConnection;
  public bool isConnectionMandatory;
  public string label = "Label";
  public string valuePath;
  public Color color;
}

public class FastNoiseSimpleInput : FastNoiseInput {
  public FastNoiseSimpleInput(string label, string valuePath) {
    this.acceptConnection = false;
    this.label = label;
    this.valuePath = valuePath;
    this.color = new Color(0.02f, 0.188f, 1f);
  }
}

public class FastNoiseHybridInput : FastNoiseInput {
  public FastNoiseHybridInput(string label, string valuePath) {
    this.acceptConnection = true;
    this.label = label;
    this.valuePath = valuePath;
    this.color = new Color(0.02f, 0.188f, 1f);
  }
}

public class FastNoiseNodeInput : FastNoiseInput {
  public FastNoiseNodeInput(string label, bool isConnectionMandatory) {
    this.acceptConnection = true;
    this.isConnectionMandatory = isConnectionMandatory;
    this.label = label;
    this.valuePath = null;
    this.color = new Color(0.02f, 0.188f, 1f);
  }
}

public class FastNoiseOutputInput : FastNoiseInput {
  public FastNoiseOutputInput(string label) {
    this.acceptConnection = true;
    this.label = label;
    this.valuePath = null;
    this.color = new Color(0.02f, 0.188f, 1f);
  }
}