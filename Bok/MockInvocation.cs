namespace Bok;

public class MockInvocation
{
    public string Name { get; set; }
    public MethodInfo Target { get; set; }
    public object[] Args { get; set; }

}
