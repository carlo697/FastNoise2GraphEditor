using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

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
        List<NoiseNode> originalNodes = new List<NoiseNode>();
        foreach (var nodeGuid in data.nodeGuids) {
          NoiseNodeView nodeView = FindNodeView(nodeGuid);
          if (nodeView != null) {
            originalNodes.Add(nodeView.node);
          }
        }

        // Copy the nodes and select them
        ClearSelection();
        Dictionary<NoiseNode, NoiseNode> newNodes = new();
        foreach (var originalNode in originalNodes) {
          NoiseNodeView newView = CopyNode(originalNode);
          newNodes.Add(originalNode, newView.node);
          AddToSelection(newView);
        }

        // Copy the edges
        foreach (var originalNode in originalNodes) {
          NoiseNode parent = originalNode;
          NoiseNode newParent = newNodes[parent];

          foreach (var edge in originalNode.edges) {
            NoiseNode child = edge.childNode;
            NoiseNode newChild;

            // Make sure the edge is connected to a node in the selection
            if (newNodes.TryGetValue(child, out newChild)) {
              newParent.edges.Add(new NoiseEdge(edge.parentPortName, newChild));
              continue;
            }
          }

          // Create the edges in the UI
          CreateEdges(newParent);
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

      // Clear the graph
      DeleteElements(graphElements);

      graphViewChanged += OnGraphViewChanged;

      // Create the nodes
      foreach (var node in tree.nodes) {
        CreateNodeView(node);
      }

      // Create the edges
      foreach (var node in tree.nodes) {
        CreateEdges(node);
      }
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change) {
      if (change.elementsToRemove != null) {
        foreach (var elementToRemove in change.elementsToRemove) {
          NoiseNodeView nodeView = elementToRemove as NoiseNodeView;
          if (nodeView != null) {
            RemoveNodeFromTree(nodeView.node);
          }

          Edge edge = elementToRemove as Edge;
          if (edge != null) {
            NoiseNodeView parentView = (NoiseNodeView)edge.input.node;
            NoiseNode parent = parentView.node;
            NoiseNodeView childView = (NoiseNodeView)edge.input.node;
            NoiseNode child = childView.node;
            string portName = parentView.namesByPort[edge.input];

            int indexInList = parent.edges.FindIndex((edge) => edge.parentPortName == portName);

            Undo.RecordObject(tree, "FastNoise Tree (Remove edge)");
            parent.edges.RemoveAt(indexInList);
            EditorUtility.SetDirty(tree);

            parentView.UpdateFieldsVisibility();
          }
        }
      }

      if (change.edgesToCreate != null) {
        foreach (var edge in change.edgesToCreate) {
          NoiseNodeView parentView = (NoiseNodeView)edge.input.node;
          NoiseNode parent = parentView.node;
          NoiseNodeView childView = (NoiseNodeView)edge.output.node;
          NoiseNode child = childView.node;
          string parentPortName = parentView.namesByPort[edge.input];

          Undo.RecordObject(tree, "FastNoise Tree (Add edge)");
          parent.edges.Add(new NoiseEdge(parentPortName, child));
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

      if (tree && evt.target == this) {
        // Get the available types of nodes
        var nodeTypes = TypeCache.GetTypesDerivedFrom<NoiseNode>();

        // Iterate the metadata to append buttons (to the menu) for adding the nodes
        foreach (var item in FastNoise.nodeMetadata) {
          // Find the path in the menu
          string menuName;
          string group = NoiseNode.nodeGroups.GetValueOrDefault(item.name);
          if (group != null) {
            menuName = $"{group}/{item.name}";
          } else {
            menuName = item.name;
          }

          // Add the button
          evt.menu.AppendAction(menuName, (action) => {
            // Create the node and the view
            NoiseNodeView view = CreateNode(item.name);

            // Move the node to the mouse
            view.SetPosition(new Rect(graphPosition, view.contentRect.size));

            // Select the node
            ClearSelection();
            AddToSelection(view);
          });
        }
      }
    }

    private NoiseNode AddNodeToTree(string name) {
      Undo.RecordObject(tree, "FastNoise Tree (Add Node)");
      NoiseNode node = tree.AddNode(name);
      EditorUtility.SetDirty(tree);
      return node;
    }

    private void RemoveNodeFromTree(NoiseNode node) {
      Undo.RecordObject(tree, "FastNoise Tree (Delete Node)");
      tree.RemoveNode(node);
      EditorUtility.SetDirty(tree);
    }

    private NoiseNodeView CopyNode(NoiseNode node) {
      NoiseNode copy = AddNodeToTree(node.metadataName);

      // Move the node a little bit
      copy.nodePosition = node.nodePosition + new Vector2(20, 20);

      // Copy the fields
      copy.intValues = new(node.intValues);
      copy.floatValues = new(node.floatValues);
      copy.enumValues = new(node.enumValues);

      return CreateNodeView(copy);
    }

    private NoiseNodeView CreateNode(string name) {
      NoiseNode node = AddNodeToTree(name);
      return CreateNodeView(node);
    }

    private NoiseNodeView CreateNodeView(NoiseNode node) {
      var nodeView = new NoiseNodeView(node, tree, this);
      AddElement(nodeView);
      return nodeView;
    }

    private void CreateEdges(NoiseNode node) {
      foreach (var fastNoiseEdge in node.edges) {
        NoiseNodeView parent = FindNodeView(node);
        NoiseNodeView child = FindNodeView(fastNoiseEdge.childNode);

        string portName = fastNoiseEdge.parentPortName;
        Edge edge = parent.portsByName[portName].ConnectTo(child.output);
        AddElement(edge);

        parent.RefreshExpandedState();
        child.RefreshExpandedState();

        parent.UpdateFieldsVisibility();
      }
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