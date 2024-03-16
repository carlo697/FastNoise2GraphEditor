using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace FastNoiseGraph.UI {
  public class FastNoiseTreeEditor : EditorWindow {
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private FastNoiseTreeView treeView;

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line) {
      if (Selection.activeObject is FastNoiseTree) {
        OpenWindow();
        return true;
      }

      return false;
    }

    [MenuItem("Window/FastNoiseTreeEditor")]
    public static void OpenWindow() {
      FastNoiseTreeEditor wnd = GetWindow<FastNoiseTreeEditor>();
      wnd.titleContent = new GUIContent("FastNoiseTreeEditor");
    }

    public void CreateGUI() {
      // Each editor window contains a root VisualElement object
      VisualElement root = rootVisualElement;

      // Instantiate UXML
      m_VisualTreeAsset.CloneTree(root);

      treeView = root.Q<FastNoiseTreeView>();

      OnSelectionChange();
    }

    private void OnSelectionChange() {
      if (
        Selection.activeObject is FastNoiseTree tree
        && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())
      ) {
        treeView.PopulateView(tree);
      }
    }
  }
}