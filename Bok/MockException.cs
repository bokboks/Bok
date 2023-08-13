namespace bok;

public class MockException : System.Exception
{
    public MockException()
    { }
    public MockException(string message) : base(message) { }
    public MockException(string message, System.Exception inner) : base(message, inner) { }
}
