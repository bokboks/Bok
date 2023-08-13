namespace bok;

public class MockMethod
{
    public string Name { get; set; }
    public MethodInfo Method { get; set; }
    public object Value { get; set; } = default;

    public long Hits { get; set; } = 0;
}
