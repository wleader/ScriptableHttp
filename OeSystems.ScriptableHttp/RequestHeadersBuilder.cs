using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IRequestHeadersBuilder
{
    void Build(HttpRequestHeaders headers, IReadOnlyValues values, IList<RequestHeaderConfig> config);
}

public class RequestHeadersBuilder : IRequestHeadersBuilder
{
    public void Build(HttpRequestHeaders headers, IReadOnlyValues values, IList<RequestHeaderConfig> config)
    {
        throw new NotImplementedException();
    }
}