using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace FastNoise2Graph.UI {
  public class NoiseNodeView : Node {
    public NoiseNode node;
    public NoiseTree tree;
    public NoiseTreeView treeView;
    public Dictionary<string, PropertyField> fieldsByName = new();
    public Dictionary<string, Port> portsByName = new();
    public Dictionary<Port, string> namesByPort = new();
    public Port output;

    public override bool expanded {
      get {
        if (node != null) {
          return !node.isCollapsed;
        }

        return base.expanded;
      }
      set {
        if (node != null) {
          node.isCollapsed = !value;
          EditorUtility.SetDirty(tree);
          RefreshExpandedState();
        }

        base.expanded = value;
      }
    }

    private Texture2D previewTexture;

    public NoiseNodeView(NoiseNode node, NoiseTree tree, NoiseTreeView treeView) {
      this.node = node;
      this.tree = tree;
      this.treeView = treeView;

      // Get the name of the node
      this.title = node.metadataName;

      this.viewDataKey = node.guid;
      this.titleContainer.style.backgroundColor = Color.black;

      this.style.width = NoiseNode.defaultNodeWidths.GetValueOrDefault(node.metadataName, 200);

      if (NoiseTree.IsOutputNode(node)) {
        capabilities = Capabilities.Selectable
          | Capabilities.Movable
          | Capabilities.Snappable;
      } else {
        capabilities = Capabilities.Selectable
          | Capabilities.Movable
          | Capabilities.Deletable
          | Capabilities.Snappable
          | Capabilities.Collapsible
          | Capabilities.Copiable;
      }

      // Set the saved position
      style.left = node.nodePosition.x;
      style.top = node.nodePosition.y;

      // Create ports
      CreateInputPorts();

      // Create preview
      CreatePreview();
      UpdatePreview();

      generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc) {
      m_initialized = true;
    }

    public override void SetPosition(Rect newPos) {
      base.SetPosition(newPos);

      Undo.RecordObject(tree, "FastNoise Tree (Set node position)");

      // Save the position
      node.nodePosition = newPos.position;

      EditorUtility.SetDirty(tree);
    }

    private (SerializedObject, SerializedProperty) GetSerializedProperty() {
      int nodeIndex = tree.nodes.IndexOf(node);
      var serializedTree = new SerializedObject(tree);
      var serializedNodes = serializedTree.FindProperty("nodes");
      var serializedNode = serializedNodes.GetArrayElementAtIndex(nodeIndex);
      return (serializedTree, serializedNode);
    }

    private bool m_initialized;

    private void CreateInputPorts() {
      var (serializedTree, serializedNode) = GetSerializedProperty();

      if (NoiseTree.IsOutputNode(node)) {
        string name = "Output";
        Port port = InstantiatePort(
          Orientation.Horizontal,
          Direction.Input,
          Port.Capacity.Single,
          typeof(float)
        );

        port.portColor = new Color(0.02f, 0.188f, 1f);
        port.portName = name;

        // Add the port
        inputContainer.Add(port);
        portsByName.Add(name, port);
        namesByPort.Add(port, name);
      } else {
        // Metadata
        FastNoise.Metadata metadata = FastNoise.GetMetadata(node.metadataName);

        // Iterate the inputs to create the ports and fields
        foreach (var (memberName, member) in metadata.members) {
          Port port = null;

          // Create a port if the input accepts connections
          if (
            member.type == FastNoise.Metadata.Member.Type.NodeLookup ||
            member.type == FastNoise.Metadata.Member.Type.Hybrid
          ) {
            port = InstantiatePort(
              Orientation.Horizontal,
              Direction.Input,
              Port.Capacity.Single,
              typeof(float)
            );

            port.portColor = new Color(0.02f, 0.188f, 1f);
            port.portName = memberName;
          }

          // Create a field if the input supports it
          if (member.type == FastNoise.Metadata.Member.Type.Int ||
            member.type == FastNoise.Metadata.Member.Type.Float ||
            member.type == FastNoise.Metadata.Member.Type.Hybrid ||
            member.type == FastNoise.Metadata.Member.Type.Enum
          ) {
            // Create a value for this field if necessary
            int valueIndex = node.EnsureMemberExists(member);
            serializedTree.UpdateIfRequiredOrScript();

            // Get the name of the array field where the value is
            string valuesFieldName;
            switch (member.type) {
              case FastNoise.Metadata.Member.Type.Int:
                valuesFieldName = "intValues";
                break;
              case FastNoise.Metadata.Member.Type.Float:
              case FastNoise.Metadata.Member.Type.Hybrid:
                valuesFieldName = "floatValues";
                break;
              default:
                valuesFieldName = "enumValues";
                break;
            }

            // Get a SerializedProperty of the member
            SerializedProperty serializedValues = serializedNode.FindPropertyRelative(valuesFieldName);
            var serializedMember = serializedValues.GetArrayElementAtIndex(valueIndex);

            // Get a SerializedProperty for the actual value inside the member
            SerializedProperty serializedMemberValue = serializedMember.FindPropertyRelative("value");

            VisualElement fieldElement;
            switch (member.type) {
              case FastNoise.Metadata.Member.Type.Enum:
                // Create the field
                DropdownField dropdown = new(memberName, new List<string>(member.enumNames), 0);

                // Binding
                dropdown.BindProperty(serializedMemberValue);

                // Register to the value changed callback
                dropdown.RegisterValueChangedCallback(evt => {
                  UpdatePreviewsRecursively();
                });

                // We add this element to make the USS class work correctly
                VisualElement fieldParent = new VisualElement();
                fieldParent.Add(dropdown);

                fieldElement = fieldParent;
                break;
              default:
                // Create the field
                PropertyField field = new PropertyField(serializedMemberValue, memberName);

                // Binding
                field.Bind(serializedTree);

                field.RegisterCallback<SerializedPropertyChangeEvent>((evt) => {
                  UpdatePreviewsRecursively();
                });

                fieldElement = field;
                fieldsByName.Add(memberName, field);

                break;
            }

            // Style field
            fieldElement.AddToClassList("node-input-property");
            if (port == null) {
              fieldElement.AddToClassList("node-input-property--no-port");
            }

            // Add the field
            if (port != null) {
              port.portName = "";
              port.contentContainer.Add(fieldElement);
            } else {
              extensionContainer.Add(fieldElement);
            }
          }

          // Add the port
          if (port != null) {
            inputContainer.Add(port);
            portsByName.Add(memberName, port);
            namesByPort.Add(port, memberName);
          }
        }

        // Create the output port
        Port outputPort = InstantiatePort(
          Orientation.Horizontal,
          Direction.Output,
          Port.Capacity.Multi,
          typeof(float)
        );
        outputPort.portName = "";
        outputPort.portColor = Color.blue;
        outputContainer.Add(outputPort);
        output = outputPort;
      }

      RefreshPorts();
      RefreshExpandedState();
    }

    public void UpdateFieldsVisibility() {
      foreach (var (memberName, field) in fieldsByName) {
        bool enabled = true;
        foreach (var edge in node.edges) {
          if (edge.parentPortName == memberName) {
            enabled = false;
          }
        }
        field.SetEnabled(enabled);
      }
    }

    ~NoiseNodeView() {
      if (previewTexture != null) {
        Texture2D.DestroyImmediate(previewTexture);
      }
    }

    public void CreatePreview() {
      if (NoiseTree.IsOutputNode(node)) {
        return;
      }

      Box box = new Box();
      box.style.width = 200;
      box.style.height = 200;

      // Noise image preview
      previewTexture = new Texture2D(200, 200);
      previewTexture.hideFlags = HideFlags.HideAndDontSave;
      box.style.backgroundImage = previewTexture;

      // Add the box
      mainContainer.Add(box);

      // Settings field
      var (serializedTree, serializedNode) = GetSerializedProperty();
      SerializedProperty serializedSettings = serializedNode.FindPropertyRelative("previewSettings");
      PropertyField field = new PropertyField(serializedSettings, "Preview");

      // Binding
      field.Bind(serializedTree);
      field.RegisterCallback<SerializedPropertyChangeEvent>((evt) => {
        UpdatePreviewsRecursively();
      });

      // Field style
      field.AddToClassList("node-input-property");

      // Add the field
      mainContainer.Add(field);
    }

    private static bool IsNodeDescendantOf(NoiseNode node, NoiseNode parent) {
      if (node == parent) {
        return true;
      }

      bool isPart = false;
      foreach (var edge in node.edges) {
        if (IsNodeDescendantOf(edge.childNode, parent)) {
          isPart = true;
          break;
        }
      }

      return isPart;
    }

    private List<NoiseNode> GetParents() {
      List<NoiseNode> parents = new();
      foreach (var treeNode in tree.nodes) {
        if (treeNode == node) {
          continue;
        }

        if (IsNodeDescendantOf(treeNode, node)) {
          parents.Add(treeNode);
        }
      }
      return parents;
    }

    public void UpdatePreviewsRecursively() {
      if (m_initialized) {
        // Update this preview
        UpdatePreview();

        // Find the parents and parent's parent, ect. of this node
        List<NoiseNode> nodesToUpdate = GetParents();

        // Update their preview
        foreach (var node in nodesToUpdate) {
          NoiseNodeView view = treeView.FindNodeView(node);
          view.UpdatePreview();
        }
      }
    }

    public void UpdatePreview() {
      if (!previewTexture) {
        return;
      }

      int resolution = 200;
      FastNoise instancedNoise = tree.GetFastNoise(node);

      if (instancedNoise != null) {
        try {
          // System.Diagnostics.Stopwatch watch = new();
          // watch.Start();

          // Generate the noise
          float[] values = new float[resolution * resolution];
          instancedNoise.GenUniformGrid2D(
            values,
            -resolution / 2,
            -resolution / 2,
            resolution, resolution,
            0.03f,
            0
          );

          // watch.Stop();
          // Debug.Log($"Noise: {watch.Elapsed.TotalMilliseconds}");

          // watch.Restart();

          // Convert the noise data into colors
          Color[] colors = new Color[resolution * resolution];
          for (int i = 0; i < colors.Length; i++) {
            // Get value in range -1 to 1
            float value = values[i];

            // Convert the value to range 0 to 1
            if (node.previewSettings.mode == NodePreviewMode.Minus1ToOne) {
              value = NoiseTextureUtils.Normalize(value);
            } else {
              value = Mathf.Clamp01(value);
            }

            // Store the final black and white color
            Color finalColor = new Color(value, value, value, 1f);
            colors[i] = finalColor;
          }

          // Set the pixels
          previewTexture.SetPixels(colors);

          // watch.Stop();
          // Debug.Log($"Set: {watch.Elapsed.TotalMilliseconds}");
        } catch (System.Exception e) {
          Debug.LogError(e);
        }
      } else {
        for (int y = 0; y < resolution; y++) {
          for (int x = 0; x < resolution; x++) {
            previewTexture.SetPixel(x, y, Color.black);
          }
        }
      }

      previewTexture.Apply();
    }
  }
}