using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace FastNoise2Graph.UI {
  public class NoiseTreeEditor : EditorWindow {
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private NoiseTree m_currentTree;
    private NoiseTreeView m_treeView;

    private void OnEnable() {
      EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
    }

    private void HandlePlayModeStateChanged(PlayModeStateChange state) {
      if (state == PlayModeStateChange.EnteredEditMode) {
        ReloadActiveTree();
      }
    }

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

    [MenuItem("Window/FastNoiseTreeEditor")]
    public static NoiseTreeEditor OpenWindow() {
      NoiseTreeEditor editor = GetWindow<NoiseTreeEditor>();
      editor.titleContent = new GUIContent("FastNoiseTreeEditor");
      return editor;
    }

    public void CreateGUI() {
      // Each editor window contains a root VisualElement object
      VisualElement root = rootVisualElement;

      // Instantiate UXML
      m_VisualTreeAsset.CloneTree(root);

      m_treeView = root.Q<NoiseTreeView>();

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