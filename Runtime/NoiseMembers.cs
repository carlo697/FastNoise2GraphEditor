namespace FastNoise2Graph {
  [System.Serializable]
  public struct NoiseIntMember {
    public string name;
    public int value;

    public NoiseIntMember(string name, int value) {
      this.name = name;
      this.value = value;
    }
  }

  [System.Serializable]
  public struct NoiseFloatMember {
    public string name;
    public float value;

    public NoiseFloatMember(string name, float value) {
      this.name = name;
      this.value = value;
    }
  }

  [System.Serializable]
  public struct NoiseStringMember {
    public string name;
    public string value;

    public NoiseStringMember(string name, string value) {
      this.name = name;
      this.value = value;
    }
  }
}