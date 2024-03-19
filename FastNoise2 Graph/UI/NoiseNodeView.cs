using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;
using FastNoise2Graph.Nodes;

namespace FastNoise2Graph.UI {
  public class NoiseNodeView : UnityEditor.Experimental.GraphView.Node {
    public NoiseNode node;
    public NoiseTree tree;
    public NoiseTreeView treeView;
    public Dictionary<int, Port> portsByIndex = new();
    public Dictionary<NoiseInput, PropertyField> fieldsByInput = new();
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
      string name = NoiseNode.GetNodeName(node);
      this.title = name;

      this.viewDataKey = node.guid;
      this.titleContainer.style.backgroundColor = node.headerBackgroundColor;

      this.style.width = node.nodeWidth;

      capabilities = node.capabilities;

      // Set the saved position
      style.left = node.nodePosition.x;
      style.top = node.nodePosition.y;

      // Create ports
      CreateInputPorts();

      // Create preview
      CreatePreview();

      generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc) {
      UpdatePreview();
    }

    public override void SetPosition(Rect newPos) {
      base.SetPosition(newPos);

      Undo.RecordObject(tree, "FastNoise Tree (Set node position)");

      // Save the position
      node.nodePosition = newPos.position;

      EditorUtility.SetDirty(tree);
    }

    private void CreateInputPorts() {
      int nodeIndex = tree.nodes.IndexOf(node);
      var serializedTree = new SerializedObject(tree);
      var serializedNodes = serializedTree.FindProperty("nodes");
      var serializedNode = serializedNodes.GetArrayElementAtIndex(nodeIndex);

      // Iterate the inputs to create the ports and fields
      for (int inputIndex = 0; inputIndex < node.inputs.Length; inputIndex++) {
        NoiseInput input = node.inputs[inputIndex];

        Port port = null;

        // Create a port if the input accepts connections
        if (input.acceptsEdge) {
          port = InstantiatePort(
            Orientation.Horizontal,
            Direction.Input,
            Port.Capacity.Single,
            typeof(float)
          );

          port.portColor = input.color;
          port.portName = input.label;
        }

        // Create a field if the input supports it
        if (input.fieldPath != null) {
          // Get a SerializedProperty
          SerializedProperty property = serializedNode.FindPropertyRelative(input.fieldPath);

          // Create the field and bind it to the SerializedObject
          PropertyField field = new PropertyField(property);
          field.Bind(serializedTree);

          // Style field
          field.AddToClassList("node-input-property");
          if (port == null) {
            field.AddToClassList("node-input-property--no-port");
          }

          field.RegisterCallback<SerializedPropertyChangeEvent>((evt) => {
            foreach (var node in tree.nodes) {
              NoiseNodeView nodeView = treeView.FindNodeView(node);
              nodeView.MarkDirtyRepaint();
            }
          });

          // Add the field
          fieldsByInput.Add(input, field);
          if (port != null) {
            port.portName = "";
            port.contentContainer.Add(field);
          } else {
            extensionContainer.Add(field);
          }
        }

        // Add the port
        if (port != null) {
          inputContainer.Add(port);
          portsByIndex.Add(inputIndex, port);
        }
      }

      if (node is not OutputNode) {
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
      // Iterate the inputs to create the ports and fields
      for (int inputIndex = 0; inputIndex < node.inputs.Length; inputIndex++) {
        NoiseInput input = node.inputs[inputIndex];

        if (input.acceptsEdge && input.fieldPath != null) {
          // Know if the input has an edge connected to it
          NoiseEdge edge = node.edges.Find((edge) => edge.parentPortIndex == inputIndex);
          bool hasEdge = edge.childNode != null;

          // Get the PropertyField of the input
          PropertyField field = fieldsByInput[input];
          if (field != null) {
            field.SetEnabled(!hasEdge);
          }
        }
      }
    }

    ~NoiseNodeView() {
      if (previewTexture != null) {
        Texture2D.DestroyImmediate(previewTexture);
      }
    }

    public void CreatePreview() {
      if (node is OutputNode) {
        return;
      }

      Box box = new Box();
      box.style.width = 200;
      box.style.height = 200;

      // Noise image preview
      previewTexture = new Texture2D(200, 200);
      box.style.backgroundImage = previewTexture;

      // Add the box
      mainContainer.Add(box);
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
          instancedNoise.GenUniformGrid2D(values, 0, 0, resolution, resolution, 0.03f, 0);

          // watch.Stop();
          // Debug.Log($"Noise: {watch.Elapsed.TotalMilliseconds}");

          // watch.Restart();

          // Convert the noise data into colors
          Color[] colors = new Color[resolution * resolution];
          for (int i = 0; i < colors.Length; i++) {
            // Get value in range -1 to 1
            float value = values[i];

            // Convert the value to range 0 to 1
            float normalizedValue = NoiseTextureUtils.Normalize(value);

            // Store the final black and white color
            Color finalColor = new Color(normalizedValue, normalizedValue, normalizedValue, 1f);
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