using UnityEngine;

namespace FastNoiseGraph {
  [System.Serializable]
  public struct FastNoiseEdge {
    public int parentPortIndex;
    [SerializeReference]
    public FastNoiseNode childNode;

    public FastNoiseEdge(int parentPortIndex, FastNoiseNode childNode) {
      this.parentPortIndex = parentPortIndex;
      this.childNode = childNode;
    }
  }
}