using UnityEngine;

namespace FastNoise2Graph {
  public abstract class NoiseInput {
    public bool acceptsEdge;
    public bool isEdgeMandatory;
    public string label = "Label";
    public string fieldPath;
    public Color color;

    public static Color defaultColor = new Color(0.02f, 0.188f, 1f);
  }

  public class NoiseSimpleInput : NoiseInput {
    public NoiseSimpleInput(string label, string fieldPath) {
      this.acceptsEdge = false;
      this.label = label;
      this.fieldPath = fieldPath;
      this.color = defaultColor;
    }
  }

  public class NoiseHybridInput : NoiseInput {
    public NoiseHybridInput(string label, string fieldPath) {
      this.acceptsEdge = true;
      this.label = label;
      this.fieldPath = fieldPath;
      this.color = defaultColor;
    }
  }

  public class NoiseNodeInput : NoiseInput {
    public NoiseNodeInput(string label, bool isEdgeMandatory) {
      this.acceptsEdge = true;
      this.isEdgeMandatory = isEdgeMandatory;
      this.label = label;
      this.fieldPath = null;
      this.color = defaultColor;
    }
  }

  public class NoiseOutputInput : NoiseInput {
    public NoiseOutputInput(string label) {
      this.acceptsEdge = true;
      this.label = label;
      this.fieldPath = null;
      this.color = defaultColor;
    }
  }
}