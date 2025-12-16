using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Regex;

namespace OeSystems.ScriptableHttp.Request;

public interface IUriBuilder
{
    System.Uri Build(IReadOnlyValues values, UriConfig config);
}

/// <summary>
/// Creates a URI from the Operation employing substitutions from the values.
/// </summary>
public class UriBuilder(IRegexMapper mapper) : IUriBuilder
{
    public System.Uri Build(IReadOnlyValues values, UriConfig config)
    {
        return new(mapper.Replace(values, config.Regex, config.Value));
    }
}