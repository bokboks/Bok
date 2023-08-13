using System.Security.Cryptography;

namespace bok;

public interface IMock<T> where T : class
{
    T Instance { get; }
    Dictionary<string, MockMethod> Methods { get; }
    Dictionary<string, MockProperty> Properties { get; }
    List<MockInvocation> Invocations { get; }

    /// <summary> Setup default return values for all properties and methods. </summary>
    IMock<T> Defaults();


    IMock<T> Setup(Action<T> action, object result);
    IMock<T> SetupProperty(Action<T> action);
}

public class Mock<T> where T : class
{
    /// <summary> Wrap the target class in a mock proxy, but don't replace any functionality. </summary>
    /// <param name="target">The target implementation of the interface to wrap.</param>
    /// <returns></returns>
    public static IMock<T> Wrap(T target)
    {
        var proxy = MockProxy<T>.Wrap(target);
        return proxy;
    }

    /// <summary> Create a new proxy mocking the interface, with default values set for all properties and methods. </summary>
    /// <returns></returns>
    public static IMock<T> Create()
    {
        var proxy = MockProxy<T>.Create();
        proxy.Defaults();
        return proxy;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T">The interface to mock</typeparam>
internal class MockProxy<T> : DispatchProxy, IMock<T> where T : class
{
    private T wrapped = null;

    public T Instance
    {
        get { return this as T; }
        private set => wrapped = value;
    }

    public Dictionary<string, MockMethod> Methods { get; private set; } = new Dictionary<string, MockMethod>();
    public Dictionary<string, MockProperty> Properties { get; private set; } = new Dictionary<string, MockProperty>();

    public List<MockInvocation> Invocations { get; private set; } = new List<MockInvocation>();

    public MockProxy() : base()
    {
    }


    public IMock<T> Defaults()
    {
        Methods.Clear();
        Properties.Clear();
        Invocations.Clear();
        PopulateMethods(typeof(T));
        return this;
    }


    private void PopulateMethods(Type type, bool checkInheritedInterfaces = true)
    {
        // set all methods to return default values
        MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (methodInfos != null && methodInfos.Length > 0)
        {
            foreach (var m in methodInfos)
            {
                if (IsMethod(m))
                {
                    var meth = new MockMethod() { Method = m, Name = m.Name };
                    if (IsPrimitive(m.ReturnType) || IsEnum(m.ReturnType))
                        meth.Value = CreateDefaultInstance(m.ReturnType);
                    this.Methods.TryAdd(m.ToString(), meth);
                }
            }
        }

        // set all properties to default values
        PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (propertyInfos != null && propertyInfos.Length > 0)
        {
            foreach (var p in propertyInfos)
            {
                var prop = new MockProperty() { Property = p, Name = p.Name };
                if (IsPrimitive(p.PropertyType) || IsEnum(p.PropertyType))
                    prop.Value = CreateDefaultInstance(p.PropertyType);
                this.Properties.TryAdd(p.Name, prop);
            }
        }

        // check for inherited interface attributes
        if (checkInheritedInterfaces && type.IsInterface)
        {
            var interfaces = type.GetInterfaces();
            foreach (var inter in interfaces)
            {
                if (type != inter)
                    PopulateMethods(inter, false);
            }
        }
    }

    private object CreateDefaultInstance(Type t)
    {
        if (t == typeof(void))
            return null;
        return Activator.CreateInstance(t);
    }

    internal static MockProxy<T> Wrap(T target)
    {
        if (target == null)
            throw new ArgumentNullException("target", "Target object to wrap cannot be null.");
        if (!typeof(T).IsInterface)
            throw new MockException("Type to mock must be an interface.");

        var proxy = Create<T, MockProxy<T>>() as MockProxy<T>;
        proxy.Instance = target;
        return proxy;
    }

    internal static MockProxy<T> Create()
    {
        if (!typeof(T).IsInterface)
            throw new MockException("Type to mock must be an interface.");
        var proxy = Create<T, MockProxy<T>>() as MockProxy<T>;
        return proxy;
    }

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        try
        {
            var key = targetMethod.ToString();
            this.Invocations.Add(new MockInvocation() { Target = targetMethod, Args = args, Name = key });

            object result = null;

            if (IsProperty(targetMethod))
            {
                key = targetMethod.Name.Replace("get_", "").Replace("set_", "");
                if (Properties.TryGetValue(key, out MockProperty prop))
                {
                    if (targetMethod.Name.StartsWith("get_", StringComparison.Ordinal))
                    {
                        prop.GetHits++;
                        result = prop.Value;
                    }
                    else if (targetMethod.Name.StartsWith("set_", StringComparison.Ordinal))
                    {
                        prop.SetHits++;
                        prop.Value = args[0];
                    }
                }
                else if (wrapped != null)
                {
                    result = targetMethod.Invoke(wrapped, args);
                }
                else
                {
                    throw new MockException("Could not find a mock result for method " + key);
                }
            }
            else
            {
                if (Methods.ContainsKey(key))
                {
                    Methods[key].Hits++;
                    result = Methods[key].Value;
                }
                else if (wrapped != null)
                {
                    result = targetMethod.Invoke(wrapped, args);
                }
                else
                {
                    throw new MockException("Could not find a mock result for method " + key);
                }
            }

            if (result == null && targetMethod.ReturnType != typeof(void) && (IsPrimitive(targetMethod.ReturnType) || IsEnum(targetMethod.ReturnType)))
            {
                throw new MockException("Result of null could not be returned as primitive type.");
            }

            return result;
        }
        catch (TargetInvocationException exc)
        {
            throw exc.InnerException;
        }
    }
    private bool IsEnum(Type t)
    {
        if (t != null && t.IsValueType && t.IsSealed && !t.IsClass)
            return true;
        return false;
    }
    private bool IsPrimitive(Type t)
    {
        if (t != null && t.IsPrimitive && !t.IsGenericType)
            return true;
        return false;
    }
    private bool IsProperty(MethodInfo m)
    {
        if (m.IsSpecialName && (m.Name.StartsWith("get_", StringComparison.Ordinal) || m.Name.StartsWith("set_", StringComparison.Ordinal)))
            return true;
        return false;
    }
    private bool IsMethod(MethodInfo m)
    {
        if (!IsProperty(m) && !m.IsConstructor)
            return true;
        return false;
    }


    public IMock<T> Setup(Action<T> action, object result)
    {
        var mockSetup = MockMethodSetup<T>.Create<T, MockMethodSetup<T>>() as MockMethodSetup<T>; ;
        mockSetup.Result=result;
        action(mockSetup.Instance);
        
        if (this.Methods.ContainsKey(mockSetup.Key))
            this.Methods[mockSetup.Key].Value = result;
        else
            this.Methods.Add(mockSetup.Key, new MockMethod() {  Value = result });
        return this;
    }

    public IMock<T> SetupProperty(Action<T> action)
    {
        var mockSetup = MockPropertySetup<T>.Create<T, MockPropertySetup<T>>() as MockPropertySetup<T>; ;
        action(mockSetup.Instance);
        var result = mockSetup.Args[0];
        if (this.Properties.ContainsKey(mockSetup.Key))
            this.Properties[mockSetup.Key].Value = result;
        else
            this.Properties.Add(mockSetup.Key, new MockProperty() { Value = result });
        return this;
    }

}

/*public class MockResult
{
    public object[] Args { get; set; }

    public string Value { get; set; }
}*/

internal class MockPropertySetup<T> : DispatchProxy, IMockSetup<T> where T : class
{
    public T Instance
    {
        get { return this as T; }
    }
    public string Key { get; set; }
    public object[] Args { get; set; }

    public MockPropertySetup() : base()
    {
    }


    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        if (!targetMethod.IsSpecialName || !targetMethod.Name.StartsWith("set_"))
            throw new MockException("Target property must be a setter.");
        Key = targetMethod.Name.Replace("get_", "").Replace("set_", "");//TODO: put into helper
        Args = args;
        return null;
    }
}
internal class MockMethodSetup<T> : DispatchProxy, IMockSetup<T> where T : class
{
    public T Instance
    {
        get { return this as T; }
    }
    public string Key { get; set; }
    public object[] Args { get; set; }
    public object Result { get;  set; }

    public MockMethodSetup() : base()
    {
    }

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        Key = targetMethod.ToString();
        Args = args;
        
        return Result;
    }
}


public interface IMockSetup<T> where T : class
{
    string Key { get; set; }
    object[] Args { get; set; }
}