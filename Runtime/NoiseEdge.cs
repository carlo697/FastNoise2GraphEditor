using UnityEngine;

namespace FastNoise2Graph {
  [System.Serializable]
  public struct NoiseEdge {
    public string parentPortName;
    [SerializeReference]
    public NoiseNode childNode;

    public NoiseEdge(string parentPortName, NoiseNode childNode) {
      this.parentPortName = parentPortName;
      this.childNode = childNode;
    }
  }
}