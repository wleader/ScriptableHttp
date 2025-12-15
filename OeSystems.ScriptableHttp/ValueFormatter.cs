using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace OeSystems.ScriptableHttp;

public interface IValueFormatter
{
    string GetFormatted(IReadOnlyValues values, string key, string? format);
}

public class ValueFormatter(
    ICultureContext cultureContext) : IValueFormatter
{
    private static readonly ConcurrentDictionary<Type, ToStrings> ToStringCache = new();

    public string GetFormatted(IReadOnlyValues values, string key, string? format)
    {
        if (!values.TryGetValue(key, out var obj) || obj == null)
            return string.Empty;
        var objectType = obj.GetType();
        var toStrings = ToStringCache.GetOrAdd(objectType, t => new(t));
        return toStrings.GetFormatted(obj, format, cultureContext.Current);
    }
}

public class ToStrings(Type type)
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
    private static readonly Type[] FormatAndProviderTypes = [typeof(string), typeof(IFormatProvider)];
    private static readonly Type[] ProviderOnlyTypes = [typeof(IFormatProvider)];
    private static readonly Type[] FormatOnlyTypes = [typeof(string)];

    private readonly MethodInfo? _formatAndProvider = FindMethod(type, FormatAndProviderTypes);
    private readonly MethodInfo? _formatOnly = FindMethod(type, FormatOnlyTypes);
    private readonly MethodInfo? _providerOnly = FindMethod(type, ProviderOnlyTypes);
    private readonly MethodInfo? _noArgs = FindMethod(type, []);

    private static MethodInfo? FindMethod(Type type, Type[] arguments)
    {
        var result = type.GetMethod("ToString", Flags, arguments);
        return result?.ReturnType == typeof(string)
            ? result
            : null;
    }

    public string GetFormatted(object value, string? format, IFormatProvider? formatProvider)
    {
        if (_formatAndProvider is not null &&
            format is not null &&
            formatProvider is not null)
            return _formatAndProvider.Invoke(value, [format, formatProvider]) as string ?? string.Empty;

        if (_formatOnly is not null &&
            format is not null)
            return _formatOnly.Invoke(value, [format]) as string ?? string.Empty;

        if (_providerOnly is not null &&
            formatProvider is not null)
            return _providerOnly.Invoke(value, [formatProvider]) as string ?? string.Empty;

        // technically this one can't be null.
        // Everything inherits object which always has a .ToString()
        return _noArgs?.Invoke(value, null) as string ?? string.Empty;
    }
}