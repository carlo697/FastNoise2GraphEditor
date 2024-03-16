using UnityEngine;

namespace FastNoiseGraph {
  public abstract class FastNoiseInput {
    public bool acceptsEdge;
    public bool isEdgeMandatory;
    public string label = "Label";
    public string fieldPath;
    public Color color;

    public static Color defaultColor = new Color(0.02f, 0.188f, 1f);
  }

  public class FastNoiseSimpleInput : FastNoiseInput {
    public FastNoiseSimpleInput(string label, string fieldPath) {
      this.acceptsEdge = false;
      this.label = label;
      this.fieldPath = fieldPath;
      this.color = defaultColor;
    }
  }

  public class FastNoiseHybridInput : FastNoiseInput {
    public FastNoiseHybridInput(string label, string fieldPath) {
      this.acceptsEdge = true;
      this.label = label;
      this.fieldPath = fieldPath;
      this.color = defaultColor;
    }
  }

  public class FastNoiseNodeInput : FastNoiseInput {
    public FastNoiseNodeInput(string label, bool isEdgeMandatory) {
      this.acceptsEdge = true;
      this.isEdgeMandatory = isEdgeMandatory;
      this.label = label;
      this.fieldPath = null;
      this.color = defaultColor;
    }
  }

  public class FastNoiseOutputInput : FastNoiseInput {
    public FastNoiseOutputInput(string label) {
      this.acceptsEdge = true;
      this.label = label;
      this.fieldPath = null;
      this.color = defaultColor;
    }
  }
}