using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FastNoiseGraph.UI {
  public class FastNoiseTreeView : GraphView {
    private FastNoiseTree tree;

    public new class UxmlFactory : UxmlFactory<FastNoiseTreeView, GraphElement.UxmlTraits> { }

    public FastNoiseTreeView() {
      // Add stylesheet
      StyleSheet styleSheet = Resources.Load<StyleSheet>("FastNoiseTreeEditor");
      styleSheets.Add(styleSheet);

      // Add background
      Insert(0, new GridBackground());

      this.AddManipulator(new ContentZoomer());
      this.AddManipulator(new ContentDragger());
      this.AddManipulator(new SelectionDragger());
      this.AddManipulator(new RectangleSelector());

      RegisterCallback<AttachToPanelEvent>((evt) => {
        Undo.undoRedoPerformed += OnUndoRedo;
      });

      RegisterCallback<DetachFromPanelEvent>((evt) => {
        Undo.undoRedoPerformed -= OnUndoRedo;
      });
    }

    private void OnUndoRedo() {
      if (tree != null) {
        PopulateView(tree);
        // AssetDatabase.SaveAssets();
      }
    }

    public void PopulateView(FastNoiseTree tree) {
      this.tree = tree;

      graphViewChanged -= OnGraphViewChanged;
      DeleteElements(graphElements);
      graphViewChanged += OnGraphViewChanged;

      foreach (var node in tree.nodes) {
        CreateNodeView(node);
      }

      foreach (var node in tree.nodes) {
        foreach (var fastNoiseEdge in node.edges) {
          FastNoiseNodeView parent = FindNodeView(node);
          FastNoiseNodeView child = FindNodeView(fastNoiseEdge.childNode);

          int portIndex = fastNoiseEdge.parentPortIndex;
          Edge edge = parent.portsByIndex[portIndex].ConnectTo(child.output);
          AddElement(edge);
        }
      }
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change) {
      if (change.elementsToRemove != null) {
        foreach (var elementToRemove in change.elementsToRemove) {
          FastNoiseNodeView nodeView = elementToRemove as FastNoiseNodeView;
          if (nodeView != null) {
            tree.DeleteNode(nodeView.node);
          }

          Edge edge = elementToRemove as Edge;
          if (edge != null) {
            int index = edge.input.parent.IndexOf(edge.input);
            FastNoiseNode parent = ((FastNoiseNodeView)edge.input.node).node;
            FastNoiseNode child = ((FastNoiseNodeView)edge.input.node).node;

            int indexInList = parent.edges.FindIndex((edge) => edge.parentPortIndex == index);

            Undo.RecordObject(tree, "FastNoise Tree (Remove edge)");
            parent.edges.RemoveAt(indexInList);
            EditorUtility.SetDirty(tree);
          }
        }
      }

      if (change.edgesToCreate != null) {
        foreach (var edge in change.edgesToCreate) {
          int index = edge.input.parent.IndexOf(edge.input);
          FastNoiseNode parent = ((FastNoiseNodeView)edge.input.node).node;
          FastNoiseNode child = ((FastNoiseNodeView)edge.output.node).node;

          Undo.RecordObject(tree, "FastNoise Tree (Add edge)");
          parent.edges.Add(new FastNoiseEdge(index, child));
          EditorUtility.SetDirty(tree);
        }
      }

      foreach (var node in tree.nodes) {
        FastNoiseNodeView nodeView = FindNodeView(node);
        nodeView.UpdatePreview();
      }

      return change;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
      base.BuildContextualMenu(evt);

      if (tree) {
        // Get the available types of nodes
        var nodeTypes = TypeCache.GetTypesDerivedFrom<FastNoiseNode>();

        // Iterate the types to append buttons (to the menu) for adding the nodes
        foreach (var nodeType in nodeTypes) {
          if (nodeType != typeof(OutputNode)) {
            // Get the name of the node
            string name = FastNoiseNode.GetNodeMenuName(nodeType);

            // Add the button
            evt.menu.AppendAction(name, (action) => {
              CreateNode(nodeType);
            });
          }
        }
      }
    }

    private void CreateNode(Type type) {
      FastNoiseNode node = tree.AddNode(type);
      CreateNodeView(node);
    }

    private void CreateNodeView(FastNoiseNode node) {
      var nodeView = new FastNoiseNodeView(node, tree, this);
      AddElement(nodeView);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
      return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    public FastNoiseNodeView FindNodeView(FastNoiseNode node) {
      return GetNodeByGuid(node.guid) as FastNoiseNodeView;
    }
  }
}