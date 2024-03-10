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
        // FloatField field = new FloatField(input.label);
        // field.labelElement.style.width = 40;
        // field.labelElement.style.flexShrink = 1;
        field.style.width = 200;
        field.style.flexGrow = 0;
        field.style.flexShrink = 1;

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
      Box box = new Box();
      box.style.width = 200;
      box.style.height = 200;

      // Background colors
      box.style.backgroundColor = Color.white;
      mainContainer.style.backgroundColor = Color.black;

      // Noise image preview
      previewTexture = new Texture2D(256, 256);
      UpdatePreview();
      box.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
      box.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
      box.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
      box.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
      box.style.backgroundImage = previewTexture;

      // Add the box
      mainContainer.Add(box);
    }
  }

  public void UpdatePreview() {
    if (!previewTexture) {
      return;
    }

    int resolution = 256;
    FastNoise instancedNoise = tree.GetFastNoise(node);

    if (instancedNoise != null) {
      try {
        float[] values = new float[resolution * resolution];
        instancedNoise.GenUniformGrid2D(values, 0, 0, resolution, resolution, 0.03f, 0);

        for (int y = 0; y < resolution; y++) {
          for (int x = 0; x < resolution; x++) {
            int index2d = TextureUtils.GetIndexFrom2d(x, y, resolution);

            float value = values[index2d];
            float normalizedValue = TextureUtils.Normalize(value);

            Color finalColor = Color.Lerp(Color.black, Color.white, normalizedValue);
            previewTexture.SetPixel(x, y, finalColor);
          }
        }
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
