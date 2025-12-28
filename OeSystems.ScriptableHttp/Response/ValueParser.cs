using System;
using System.Collections.Concurrent;
using System.Reflection;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Response;

public interface IValueParser
{
    ObjectResult Parse(string? value, BaseMap map);
}

public class ValueParser(ICultureContext cultureContext) : IValueParser
{
    private static readonly ConcurrentDictionary<string, TryParses> TryParsesCache = new();

    public ObjectResult Parse(string? value, BaseMap map)
    {
        var tryParses = TryParsesCache.GetOrAdd(map.DataType, typeString => new(typeString));
        return tryParses.Type is null
            ? ObjectResult.Fail($"'{map.DataType}' was not a recognized type.")
            : tryParses.Parse(value, map, cultureContext.Current);
    }
}

public class TryParses
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Static;
    private const string TryParseMethodName = "TryParse";
    private const string TryParseExactMethodName = "TryParseExact";

    public Type? Type { get; }

    private readonly MethodInfo? _tryParse;
    private readonly MethodInfo? _tryParseFormatProvider;
    private readonly MethodInfo? _tryParseExact;
    private readonly MethodInfo? _tryParseExactFormatProvider;

    public TryParses(string typeString)
    {
        Type = Type.GetType(typeString);

        if (Type is null) return;

        var outType = Type.MakeByRefType();

        // char bool
        // int long decimal ulong double float guid timespan
        // datetime datetimeoffset
        _tryParse = FindMethod(Type, TryParseMethodName,
            typeof(string),
            outType);

        // int long decimal ulong double float guid timespan
        // datetime datetimeoffset
        _tryParseFormatProvider = FindMethod(Type, TryParseMethodName,
            typeof(string),
            typeof(IFormatProvider),
            outType);

        // guid
        _tryParseExact = FindMethod(Type, TryParseExactMethodName,
            typeof(string),
            typeof(string),
            outType);

        // timespan
        _tryParseExactFormatProvider = FindMethod(Type, TryParseExactMethodName,
            typeof(string),
            typeof(string),
            typeof(IFormatProvider),
            outType);
    }

    private static MethodInfo? FindMethod(Type type, string methodName, params Type[] arguments)
    {
        var result = type.GetMethod(methodName, Flags, arguments);
        return result?.ReturnType == typeof(string)
            ? result
            : null;
    }

    public ObjectResult Parse(string? value, BaseMap map, IFormatProvider? formatProvider)
    {
        // string doesn't have any TryParse or TryParseExact.
        if (Type == typeof(string))
            return new(value, Errors.None);

        object? output = null;

        if (_tryParseExactFormatProvider is not null &&
            !string.IsNullOrEmpty(map.Format) &&
            formatProvider is not null)
        {
            var parsed = _tryParseExactFormatProvider.Invoke(null, [value, map.Format, formatProvider, output]);
            if (parsed is not null && (bool)parsed)
                return ObjectResult.Success(output);
        }

        if (_tryParseExact is not null &&
            !string.IsNullOrEmpty(map.Format))
        {
            var parsed = _tryParseExact.Invoke(null, [value, map.Format, output]);
            if (parsed is not null && (bool)parsed)
                return ObjectResult.Success(output);
        }

        if (_tryParseFormatProvider is not null)
        {
            var parsed = _tryParseFormatProvider.Invoke(null, [value, formatProvider, output]);
            if (parsed is not null && (bool)parsed)
                return ObjectResult.Success(output);
        }

        if (_tryParse is not null)
        {
            var parsed = _tryParse.Invoke(null, [value, output]);
            if (parsed is not null && (bool)parsed)
                return ObjectResult.Success(output);
        }

        // give up
        return ObjectResult.Fail($"The value for Key '{map.Key}' could not be converted to '{map.DataType}'.");
    }
}