namespace Bok;

public class MockProperty
{
    public string Name { get; set; }
    public PropertyInfo Property { get; set; }
    public object Value { get; set; } = default;
    public long GetHits { get; set; } = 0;
    public long SetHits { get; set; } = 0;
}
