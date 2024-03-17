using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using FastNoise2Graph.Nodes;

namespace FastNoise2Graph.UI {
  public class NoiseTreeView : GraphView {
    private NoiseTree tree;

    public new class UxmlFactory : UxmlFactory<NoiseTreeView, GraphElement.UxmlTraits> { }

    [Serializable]
    private class CopyPasteData {
      public List<string> nodeGuids = new List<string>();

      public CopyPasteData(IEnumerable<GraphElement> elements) {
        foreach (var item in elements) {
          if (item is NoiseNodeView view) {
            nodeGuids.Add(view.node.guid);
          }
        }
      }
    }

    public NoiseTreeView() {
      // Add stylesheet
      StyleSheet styleSheet = Resources.Load<StyleSheet>("NoiseTreeEditor");
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

      serializeGraphElements += (IEnumerable<GraphElement> elements) => {
        CopyPasteData data = new(elements);
        string jsonData = JsonUtility.ToJson(data);
        return jsonData;
      };

      canPasteSerializedData += (jsonData) => {
        try {
          JsonUtility.FromJson<CopyPasteData>(jsonData);
          return true;
        } catch (System.Exception) {
          return false;
        }
      };

      unserializeAndPaste += (operationName, jsonData) => {
        CopyPasteData data = JsonUtility.FromJson<CopyPasteData>(jsonData);

        // Build a list of the original node views
        List<NoiseNodeView> originalNodes = new List<NoiseNodeView>();
        foreach (var nodeGuid in data.nodeGuids) {
          NoiseNodeView nodeView = FindNodeView(nodeGuid);
          if (nodeView != null) {
            originalNodes.Add(nodeView);
          }
        }

        // Copy the nodes and select them
        ClearSelection();
        foreach (var item in originalNodes) {
          NoiseNodeView nodeView = CopyNode(item.node);
          AddToSelection(nodeView);
        }
      };
    }

    private void OnUndoRedo() {
      if (tree != null) {
        PopulateView(tree);
        // AssetDatabase.SaveAssets();
      }
    }

    public void PopulateView(NoiseTree tree) {
      this.tree = tree;

      graphViewChanged -= OnGraphViewChanged;
      DeleteElements(graphElements);
      graphViewChanged += OnGraphViewChanged;

      foreach (var node in tree.nodes) {
        CreateNodeView(node);
      }

      foreach (var node in tree.nodes) {
        foreach (var fastNoiseEdge in node.edges) {
          NoiseNodeView parent = FindNodeView(node);
          NoiseNodeView child = FindNodeView(fastNoiseEdge.childNode);

          int portIndex = fastNoiseEdge.parentPortIndex;
          Edge edge = parent.portsByIndex[portIndex].ConnectTo(child.output);
          AddElement(edge);

          parent.UpdateFieldsVisibility();
        }
      }
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change) {
      if (change.elementsToRemove != null) {
        foreach (var elementToRemove in change.elementsToRemove) {
          NoiseNodeView nodeView = elementToRemove as NoiseNodeView;
          if (nodeView != null) {
            tree.DeleteNode(nodeView.node);
          }

          Edge edge = elementToRemove as Edge;
          if (edge != null) {
            int index = edge.input.parent.IndexOf(edge.input);
            NoiseNodeView parentView = (NoiseNodeView)edge.input.node;
            NoiseNode parent = parentView.node;
            NoiseNodeView childView = (NoiseNodeView)edge.input.node;
            NoiseNode child = childView.node;

            int indexInList = parent.edges.FindIndex((edge) => edge.parentPortIndex == index);

            Undo.RecordObject(tree, "FastNoise Tree (Remove edge)");
            parent.edges.RemoveAt(indexInList);
            EditorUtility.SetDirty(tree);

            parentView.UpdateFieldsVisibility();
          }
        }
      }

      if (change.edgesToCreate != null) {
        foreach (var edge in change.edgesToCreate) {
          int index = edge.input.parent.IndexOf(edge.input);
          NoiseNodeView parentView = (NoiseNodeView)edge.input.node;
          NoiseNode parent = parentView.node;
          NoiseNodeView childView = (NoiseNodeView)edge.output.node;
          NoiseNode child = childView.node;

          Undo.RecordObject(tree, "FastNoise Tree (Add edge)");
          parent.edges.Add(new NoiseEdge(index, child));
          EditorUtility.SetDirty(tree);

          parentView.UpdateFieldsVisibility();
        }
      }

      foreach (var node in tree.nodes) {
        NoiseNodeView nodeView = FindNodeView(node);
        nodeView.UpdatePreview();
      }

      return change;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
      base.BuildContextualMenu(evt);

      Vector2 mousePosition = evt.localMousePosition;
      Vector2 graphPosition = viewTransform.matrix.inverse.MultiplyPoint(mousePosition);

      if (tree && evt.target is not NoiseNodeView) {
        // Get the available types of nodes
        var nodeTypes = TypeCache.GetTypesDerivedFrom<NoiseNode>();

        // Iterate the types to append buttons (to the menu) for adding the nodes
        foreach (var nodeType in nodeTypes) {
          if (nodeType != typeof(OutputNode)) {
            // Get the name of the node
            string name = NoiseNode.GetNodeMenuName(nodeType);

            // Add the button
            evt.menu.AppendAction(name, (action) => {
              // Create the node and the view
              NoiseNodeView view = CreateNode(nodeType);

              // Move the node to the mouse
              view.SetPosition(new Rect(graphPosition, view.contentRect.size));

              // Select the node
              ClearSelection();
              AddToSelection(view);
            });
          }
        }
      }
    }

    private NoiseNodeView CopyNode(NoiseNode node) {
      Type nodeType = node.GetType();
      NoiseNode copy = tree.AddNode(nodeType);

      // Move the node a little bit
      copy.nodePosition = node.nodePosition + new Vector2(20, 20);

      // Copy the fields
      foreach (var input in node.inputs) {
        if (input.fieldPath != null) {
          FieldInfo field = nodeType.GetField(input.fieldPath);
          object originalValue = field.GetValue(node);
          field.SetValue(copy, originalValue);
        }
      }

      return CreateNodeView(copy);
    }

    private NoiseNodeView CreateNode(Type type) {
      NoiseNode node = tree.AddNode(type);
      return CreateNodeView(node);
    }

    private NoiseNodeView CreateNode(NoiseNode node) {
      NoiseNode copy = tree.AddNode(node.GetType());
      return CreateNodeView(copy);
    }

    private NoiseNodeView CreateNodeView(NoiseNode node) {
      var nodeView = new NoiseNodeView(node, tree, this);
      AddElement(nodeView);
      return nodeView;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
      return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    public NoiseNodeView FindNodeView(string guid) {
      return GetNodeByGuid(guid) as NoiseNodeView;
    }

    public NoiseNodeView FindNodeView(NoiseNode node) {
      return FindNodeView(node.guid);
    }
  }
}