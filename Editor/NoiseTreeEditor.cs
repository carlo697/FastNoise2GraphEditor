using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FastNoise2Graph.UI {
  public class NoiseTreeEditor : EditorWindow {
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField]
    private NoiseTree m_currentTree;
    private NoiseTreeView m_treeView;

    [OnOpenAsset]
    private static bool OnOpenAsset(int instanceId, int line) {
      NoiseTree tree = EditorUtility.InstanceIDToObject(instanceId) as NoiseTree;
      if (tree != null) {
        NoiseTreeEditor editor = OpenWindow(tree);
        return true;
      }

      return false;
    }

    public static NoiseTreeEditor OpenWindow(NoiseTree tree) {
      // Know if there's already a window
      NoiseTreeEditor[] windows = Resources.FindObjectsOfTypeAll<NoiseTreeEditor>();
      foreach (var window in windows) {
        if (window.m_currentTree == tree) {
          window.Focus();
          return window;
        }
      }

      // Create a new window
      NoiseTreeEditor editor = CreateWindow<NoiseTreeEditor>(typeof(SceneView));
      editor.OpenTree(tree);
      return editor;
    }

    private void CreateGUI() {
      // Each editor window contains a root VisualElement object
      VisualElement root = rootVisualElement;

      // Instantiate UXML
      m_VisualTreeAsset.CloneTree(root);

      m_treeView = root.Q<NoiseTreeView>();

      // Button to select all elements
      ToolbarButton selectAllButton = root.Q<ToolbarButton>(name: "SelectAllButton");
      selectAllButton.clicked += m_treeView.SelectAll;

      // Re-open tree if there was one
      ReloadActiveTree();
    }

    private void OpenTree(NoiseTree tree) {
      titleContent.text = tree.name;

      m_currentTree = tree;
      m_treeView.PopulateView(tree);

      EditorApplication.delayCall += () => {
        m_treeView.FrameAll();
      };
    }

    private void ReloadActiveTree() {
      if (m_currentTree != null) {
        OpenTree(m_currentTree);
      }
    }
  }
}