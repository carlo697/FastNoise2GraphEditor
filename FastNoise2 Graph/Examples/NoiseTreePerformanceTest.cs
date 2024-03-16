using UnityEngine;

namespace FastNoise2Graph.Examples {
  public class NoiseTreePerformanceTest : MonoBehaviour {
    public NoiseTree tree;

    void Update() {
      for (int i = 0; i < 100; i++) {
        FastNoise noise = tree.GetFastNoise();
      }
    }
  }
}
