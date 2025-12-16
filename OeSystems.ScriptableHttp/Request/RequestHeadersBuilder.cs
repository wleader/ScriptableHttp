using System.Collections.Generic;
using System.Net.Http.Headers;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Regex;

namespace OeSystems.ScriptableHttp.Request;

public interface IRequestHeadersBuilder
{
    void Build(HttpRequestHeaders headers, IReadOnlyValues values, IList<RequestHeaderConfig> config);
}

public class RequestHeadersBuilder(IRegexMapper regexMapper) : IRequestHeadersBuilder
{
    public void Build(HttpRequestHeaders headers, IReadOnlyValues values, IList<RequestHeaderConfig> config)
    {
        foreach (var headerConfig in config)
        {
            var value = regexMapper.Replace(values, headerConfig.Regex, headerConfig.Value);
            headers.Add(headerConfig.Name, value);
        }
    }
}