using UnityEngine;

namespace FastNoise2Graph {
  [System.Serializable]
  public struct NoiseEdge {
    public int parentPortIndex;
    [SerializeReference]
    public NoiseNode childNode;

    public NoiseEdge(int parentPortIndex, NoiseNode childNode) {
      this.parentPortIndex = parentPortIndex;
      this.childNode = childNode;
    }
  }
}