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

      treeView = root.Q<NoiseTreeView>();
    }

    public void OpenTree(NoiseTree tree) {
      treeView.PopulateView(tree);

      EditorApplication.delayCall += () => {
        treeView.FrameAll();
      };
    }
  }
}