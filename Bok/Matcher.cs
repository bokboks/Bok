using System;
using System.Diagnostics;

namespace Bok;

public class Matcher
{
    public Matcher() { }
    public bool IsMatch(object[] args, object[] matches)
    {
        if (args == null && matches == null)
        {
            return true;
        }
        else if (args.Length == 0 && matches.Length == 0)
        {
            return true;
        }
        else if (args.Length == matches.Length)
        {
            for (int i = 0; i < args.Length; i++)
            {
                object arg = args[i];
                object match = matches[i];

                if (match == null && arg == null)
                    continue;
                if (match == null && arg != null)
                    return false;
                if (match == arg)
                    continue;

                if (match != null)
                {
                    var type = match.GetType();
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Any<>) && (type.GenericTypeArguments.FirstOrDefault() == arg?.GetType() || arg==null || type.GenericTypeArguments.FirstOrDefault()==typeof(object)))
                    {
                        continue;
                    }
                    else if (type == typeof(Match<>))
                    {
                        if (((bool)(match.GetType().InvokeMember(
                            "IsMatch",
                            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                            null,
                            match,
                           new[] { arg }))))
                            continue;
                        else
                            return false;
                    }
                    if (!object.Equals(match, arg))
                    {
                        return false;
                    }
                }

            }
            return true;
        }
        return false;
    }
}

public abstract class BaseMatch<TT>
{
    public abstract bool IsMatch(TT input);
}
public static class Any
{
    public static Any<string> String { get { return new Any<string>(); } }
    public static Any<int> Int { get { return new Any<int>(); } }
    public static Any<bool> Bool { get { return new Any<bool>(); } }
    public static Any<double> Double { get { return new Any<double>(); } }
    public static Any<long> Long { get { return new Any<long>(); } }
    public static Any<DateTime> DateTime { get { return new Any<DateTime>(); } }
    public static Any<char> Char { get { return new Any<char>(); } }

    public static Any<int?> NullableInt { get { return new Any<int?>(); } }
    public static Any<bool?> NullableBool { get { return new Any<bool?>(); } }
    public static Any<double?> NullableDouble { get { return new Any<double?>(); } }
    public static Any<long?> NullableLong { get { return new Any<long?>(); } }
    public static Any<DateTime?> NullableDateTime { get { return new Any<DateTime?>(); } }
    public static Any<char?> NullableChar { get { return new Any<char?>(); } }



    public static Any<object> Thing { get { return new Any<object>(); } }
}
public class Any<TT> : BaseMatch<TT>
{
    public Any() { }
    public override bool IsMatch(TT input)
    {
        return true;
    }
}

public class Match<TT> : BaseMatch<TT> where TT : IComparable<TT>
{
    public Match(TT match)
    {
        Value = match;
    }

    public TT Value { get; }
    public override bool IsMatch(TT input)
    {
        if (object.Equals(Value, input))
            return true;
        return false;
    }
}