using UnityEngine;
using System;
using System.Collections.Generic;

namespace FastNoise2Graph {
  public enum NodePreviewMode {
    [InspectorName("-1 to 1")]
    Minus1ToOne,
    [InspectorName("0 to 1")]
    ZeroToOne
  }

  [System.Serializable]
  public class NoiseNodePreviewSettings {
    public NodePreviewMode mode;
  }

  [System.Serializable]
  public class NoiseNode {
    public string guid;
    public Vector2 nodePosition;
    public List<NoiseEdge> edges = new();
    public string metadataName;
    public bool isCollapsed;
    public NoiseNodePreviewSettings previewSettings = new();

    public List<NoiseIntMember> intValues = new();
    public List<NoiseFloatMember> floatValues = new();
    public List<NoiseStringMember> enumValues = new();

    public NoiseNode(string name) {
      this.metadataName = name;
      this.guid = Guid.NewGuid().ToString();
    }

    public int EnsureMemberExists(FastNoise.Metadata.Member member) {
      switch (member.type) {
        case FastNoise.Metadata.Member.Type.Int:
          int intDefault = defaultIntValues.GetValueOrDefault(member.name, 0);
          return EnsureIntValueExists(member.name, intDefault);
        case FastNoise.Metadata.Member.Type.Float:
        case FastNoise.Metadata.Member.Type.Hybrid:
          float floatDefault = defaultFloatValues.GetValueOrDefault(member.name, 0f);
          return EnsureFloatValueExists(member.name, floatDefault);
        case FastNoise.Metadata.Member.Type.Enum:
          string enumDefault = defaultEnumValues.GetValueOrDefault(member.name, member.enumNames[0]);
          return EnsureStringValueExists(member.name, enumDefault);
        default:
          return -1;
      }
    }

    private int EnsureIntValueExists(string name, int defaultValue) {
      for (int i = 0; i < intValues.Count; i++) {
        if (intValues[i].name == name) {
          return i;
        }
      }
      intValues.Add(new NoiseIntMember(name, defaultValue));
      return intValues.Count - 1;
    }

    private int EnsureFloatValueExists(string name, float defaultValue) {
      for (int i = 0; i < floatValues.Count; i++) {
        if (floatValues[i].name == name) {
          return i;
        }
      }
      floatValues.Add(new NoiseFloatMember(name, defaultValue));
      return floatValues.Count - 1;
    }

    private int EnsureStringValueExists(string name, string defaultValue) {
      for (int i = 0; i < enumValues.Count; i++) {
        if (enumValues[i].name == name) {
          return i;
        }
      }
      enumValues.Add(new NoiseStringMember(name, defaultValue));
      return enumValues.Count - 1;
    }

    public static Dictionary<string, string> nodeGroups = new Dictionary<string, string> {
      { "Checkerboard", "Basic Generators" },
      { "Constant", "Basic Generators" },
      { "DistanceToPoint", "Basic Generators" },
      { "PositionOutput", "Basic Generators" },
      { "SineWave", "Basic Generators" },
      { "White", "Basic Generators" },
      { "Add", "Blends" },
      { "Divide", "Blends" },
      { "Fade", "Blends" },
      { "Max", "Blends" },
      { "MaxSmooth", "Blends" },
      { "Min", "Blends" },
      { "MinSmooth", "Blends" },
      { "Multiply", "Blends" },
      { "PowFloat", "Blends" },
      { "PowInt", "Blends" },
      { "Subtract", "Blends" },
      { "CellularDistance", "Coherent Noise" },
      { "CellularLookup", "Coherent Noise" },
      { "CellularValue", "Coherent Noise" },
      { "OpenSimplex2", "Coherent Noise" },
      { "OpenSimplex2S", "Coherent Noise" },
      { "Perlin", "Coherent Noise" },
      { "Simplex", "Coherent Noise" },
      { "Value", "Coherent Noise" },
      { "DomainWarpGradient", "DomainWarp" },
      { "DomainWarpFractalIndependant", "Fractal/Domain Warp" },
      { "DomainWarpFractalProgressive", "Fractal/Domain Warp" },
      { "FractalFBm", "Fractal" },
      { "FractalPingPong", "Fractal" },
      { "FractalRidged", "Fractal" },
      { "AddDimension", "Modifiers" },
      { "ConvertRGBA8", "Modifiers" },
      { "DomainAxisScale", "Modifiers" },
      { "DomainOffset", "Modifiers" },
      { "DomainRotate", "Modifiers" },
      { "DomainScale", "Modifiers" },
      { "GeneratorCache", "Modifiers" },
      { "Remap", "Modifiers" },
      { "RemoveDimension", "Modifiers" },
      { "SeedOffset", "Modifiers" },
      { "Terrace", "Modifiers" },
    };

    public static Dictionary<string, int> defaultNodeWidths = new Dictionary<string, int> {
      { "CellularDistance", 250 },
      { "CellularLookup", 250 },
      { "CellularValue", 250 },
      { "DomainWarpGradient", 250 },
      { "DomainWarpFractalIndependant", 250 },
      { "DomainWarpFractalProgressive", 250 },
      { "FractalFBm", 250 },
      { "FractalPingPong", 250 },
      { "FractalRidged", 250 },
    };

    public static Dictionary<string, int> defaultIntValues = new Dictionary<string, int> {
      // Pow Int
      { "Pow", 2 },

      // Cellular Distance
      { "Distance Index 1", 1 },

      // Domain Warp Fractal Independant
      // { "Octaves", 3 },

      // Domain Warp Fractal Progressive
      // { "Octaves", 3 },

      // Fractal FBm
      // { "Octaves", 3 },

      // Fractal Ping Pong
      // { "Octaves", 3 },

      // Fractal Ridged
      { "Octaves", 3 },

      // Seed Offset
      { "Seed Offset", 1 },
    };

    public static Dictionary<string, float> defaultFloatValues = new Dictionary<string, float> {
      // Checkerboard
      { "Size", 1f },

      // Constant
      // { "Value", 1f },

      // Sine Wave/Domain Scale
      { "Scale", 1f },

      // Fade
      { "Fade", 0.5f },

      // Max Smooth/Min Smooth
      { "Smoothness", 0.1f },

      // Pow Float
      { "Value", 2f },
      { "Pow", 2f },

      // Cellular Distance
      { "Jitter Modifier", 1f },
      { "Lookup Frequency", 0.1f },
      
      // Cellular Lookup
      // { "Jitter Modifier", 1f },
      // { "Lookup Frequency", 0.1f },

      // Cellular Value
      // { "Jitter Modifier", 1f },

      // Domain Warp Gradient
      { "Warp Amplitude", 1f },
      { "Warp Frequency", 0.5f },

      // Domain Warp Fractal Independant
      // { "Gain", 0.5f },
      // { "Lacunarity", 2f },

      // Domain Warp Fractal Progressive
      // { "Gain", 0.5f },
      // { "Lacunarity", 2f },

      // Fractal FBm
      // { "Gain", 0.5f },
      // { "Lacunarity", 2f },

      // Fractal Ping Pong
      { "Gain", 0.5f },
      { "Ping Pong Strength", 0.5f },
      { "Lacunarity", 2f },

      // Fractal Ridged
      // { "Gain", 0.5f },
      // { "Lacunarity", 2f },

      // Convert RGBA8
      { "Min", -1f },
      { "Max", 1f },

      // Domain Axis Scale
      { "Scale X", 1f },
      { "Scale Y", 1f },
      { "Scale Z", 1f },
      { "Scale W", 1f },

      // Domain Offset
      // Domain Rotate
      
      // Remap
      { "From Min", -1f },
      { "From Max", 1f },
      { "To Min", 0f },
      { "To Max", 1f },

      // Terrace
      { "Multiplier", 1f },
      // { "Smoothness", 0f },
    };

    public static Dictionary<string, string> defaultEnumValues = new Dictionary<string, string> {
      // Distance To Point
      // { "Distance Function", "Euclidean" },
      
      // Cellular Distance
      { "Distance Function", "Euclidean Squared" },
      { "Return Type", "Index0" },

      // Cellular Lookup
      // { "Distance Function", "Euclidean Squared" },

      // Cellular Value
      // { "Distance Function", "Euclidean Squared" },

      // Remove Dimension
      { "Remove Dimension", "Y" },
    };
  }
}