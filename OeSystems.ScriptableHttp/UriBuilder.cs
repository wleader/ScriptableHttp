using Uri = OeSystems.ScriptableHttp.Configuration.Uri;

namespace OeSystems.ScriptableHttp;

public interface IUriBuilder
{
    System.Uri Build(IReadOnlyValues values, Uri config);
}

/// <summary>
/// Creates a URI from the Operation employing substitutions from the values.
/// </summary>
public class UriBuilder(IRegexMapper mapper) : IUriBuilder
{
    public System.Uri Build(IReadOnlyValues values, Uri config)
    {
        var value = config.Value;
        foreach (var mapping in  config.Regex)
        {
            value = mapper.Replace(values, mapping, value);
        }
        return new(value);
    }
}