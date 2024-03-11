using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.Reflection;

public class FastNoiseNodeView : UnityEditor.Experimental.GraphView.Node {
  public FastNoiseNode node;
  public FastNoiseTree tree;
  public FastNoiseTreeView treeView;
  public Dictionary<int, Port> portsByIndex = new();
  public Port output;

  private Texture2D previewTexture;

  public FastNoiseNodeView(FastNoiseNode node, FastNoiseTree tree, FastNoiseTreeView treeView) {
    this.node = node;
    this.tree = tree;
    this.treeView = treeView;

    // Get the name of the node
    string name = FastNoiseNode.GetNodeName(node);
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

    Undo.RecordObject(node, "FastNoise Tree (Set node position)");

    // Save the position
    node.nodePosition = newPos.position;

    EditorUtility.SetDirty(node);
  }

  private void CreateInputPorts() {
    var serializedNode = new SerializedObject(node);

    // Iterate the inputs to create the ports and fields
    for (int inputIndex = 0; inputIndex < node.inputs.Length; inputIndex++) {
      FastNoiseInput input = node.inputs[inputIndex];

      Port port = null;

      // Create a port if the input accepts connections
      if (input.acceptConnection) {
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
      if (input.valuePath != null) {
        // Get a SerializedProperty
        SerializedProperty property = serializedNode.FindProperty(input.valuePath);

        // Create the field and bind it to the SerializedObject
        PropertyField field = new PropertyField(property);
        field.Bind(serializedNode);

        // Style field
        field.AddToClassList("node-input-property");
        if (port == null) {
          field.AddToClassList("node-input-property--no-port");
        }

        field.RegisterCallback<SerializedPropertyChangeEvent>((evt) => {
          foreach (var node in tree.nodes) {
            FastNoiseNodeView nodeView = treeView.FindNodeView(node);
            nodeView.MarkDirtyRepaint();
          }
        });

        // Add the field
        if (port != null) {
          port.portName = "";
          port.contentContainer.Add(field);
        } else {
          inputContainer.Add(field);
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
  }

  ~FastNoiseNodeView() {
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

    // Background colors
    box.style.backgroundColor = Color.black;
    mainContainer.style.backgroundColor = Color.black;

    // Noise image preview
    previewTexture = new Texture2D(200, 200);
    UpdatePreview();
    box.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
    box.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
    box.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
    box.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
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
          float normalizedValue = TextureUtils.Normalize(value);

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
