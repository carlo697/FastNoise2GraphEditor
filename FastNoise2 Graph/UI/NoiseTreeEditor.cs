using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace FastNoise2Graph.UI {
  public class NoiseTreeEditor : EditorWindow {
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private NoiseTreeView treeView;

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line) {
      if (Selection.activeObject is NoiseTree) {
        OpenWindow();
        return true;
      }

      return false;
    }

    [MenuItem("Window/FastNoiseTreeEditor")]
    public static void OpenWindow() {
      NoiseTreeEditor wnd = GetWindow<NoiseTreeEditor>();
      wnd.titleContent = new GUIContent("FastNoiseTreeEditor");
    }

    public void CreateGUI() {
      // Each editor window contains a root VisualElement object
      VisualElement root = rootVisualElement;

      // Instantiate UXML
      m_VisualTreeAsset.CloneTree(root);

      treeView = root.Q<NoiseTreeView>();

      OnSelectionChange();
    }

    private void OnSelectionChange() {
      if (
        Selection.activeObject is NoiseTree tree
        && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())
      ) {
        treeView.PopulateView(tree);
      }
    }
  }
}