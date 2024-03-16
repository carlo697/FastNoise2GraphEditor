namespace FastNoise2Graph {
  [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public class NodeNameAttribute : System.Attribute {
    public string name;
    public string menuName;

    public NodeNameAttribute(string name) {
      this.name = name;
      this.menuName = name;
    }

    public NodeNameAttribute(string name, string menuName) {
      this.name = name;
      this.menuName = menuName;
    }
  }
}