using UnityEngine;

namespace FastNoiseGraph.Examples {
  public class FastNoiseTreePerformanceTest : MonoBehaviour {
    public FastNoiseTree tree;

    void Update() {
      for (int i = 0; i < 100; i++) {
        FastNoise noise = tree.GetFastNoise();
      }
    }
  }
}
