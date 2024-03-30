using UnityEngine;
using UnityEditor;
using System;

namespace FastNoise2Graph.UI {
  public class NoiseTreeModificationProcessor : AssetModificationProcessor {
    private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options) {
      // If it's a NoiseTree asset, close its editor window
      Type type = AssetDatabase.GetMainAssetTypeAtPath(path);
      if (type == typeof(NoiseTree)) {
        NoiseTree tree = AssetDatabase.LoadAssetAtPath<NoiseTree>(path);
        if (tree != null) {
          NoiseTreeEditor.CloseWindow(tree);
        }
      }

      return AssetDeleteResult.DidNotDelete;
    }
  }
}
