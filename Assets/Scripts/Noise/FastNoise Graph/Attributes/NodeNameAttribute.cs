[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class NodeNameAttribute : System.Attribute {
  public string name;

  public NodeNameAttribute(string name) {
    this.name = name;
  }
}