using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FastNoise2Graph.Examples.UI {
  [UnityEditor.InitializeOnLoad]
  static class PreventSavingNoiseTreePreview {
    private static Dictionary<MeshRenderer, Material> cachedMaterials =
      new Dictionary<MeshRenderer, Material>();

    static PreventSavingNoiseTreePreview() {
      EditorSceneManager.sceneSaved += OnSceneSaved;
      EditorSceneManager.sceneSaving += OnSceneSaving;
    }

    static void OnSceneSaving(Scene scene, string path) {
      cachedMaterials.Clear();

      var components = Resources.FindObjectsOfTypeAll<NoiseTreePreview>();
      foreach (var chunk in components) {
        MeshRenderer meshRenderer = chunk.GetComponent<MeshRenderer>();

        if (meshRenderer) {
          cachedMaterials.Add(meshRenderer, meshRenderer.sharedMaterial);
          meshRenderer.sharedMaterial = null;
        }
      }
    }

    static void OnSceneSaved(Scene scene) {
      foreach (var item in cachedMaterials) {
        item.Key.sharedMaterial = item.Value;
      }
    }
  }
}