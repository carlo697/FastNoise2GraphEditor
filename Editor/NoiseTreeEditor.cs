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
    public static bool OnOpenAsset(int instanceId, int line) {
      NoiseTree tree = EditorUtility.InstanceIDToObject(instanceId) as NoiseTree;
      if (tree != null) {
        NoiseTreeEditor editor = OpenWindow();
        editor.OpenTree(tree);
        return true;
      }

      return false;
    }

    [MenuItem("Window/FastNoise2 Graph Editor")]
    public static NoiseTreeEditor OpenWindow() {
      NoiseTreeEditor editor = GetWindow<NoiseTreeEditor>();
      editor.titleContent = new GUIContent("FastNoise2 Graph Editor");
      return editor;
    }

    public void CreateGUI() {
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

    public void OpenTree(NoiseTree tree) {
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