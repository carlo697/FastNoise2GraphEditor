using UnityEngine;
using System.Collections.Generic;

namespace FastNoise2Graph.Examples {
  public class NoiseTreePerformanceTest : MonoBehaviour {
    public NoiseTree tree;
    private Dictionary<NoiseNode, FastNoise> m_cache;

    void Update() {
      for (int i = 0; i < 1; i++) {
        FastNoise noise = tree.GetFastNoise();
      }
    }
  }
}
