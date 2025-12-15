using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IRequestHeadersBuilder
{
    void Build(HttpRequestHeaders headers, IReadOnlyValues values, IList<RequestHeader> config);
}

public class RequestHeadersBuilder : IRequestHeadersBuilder
{
    public void Build(HttpRequestHeaders headers, IReadOnlyValues values, IList<RequestHeader> config)
    {
        throw new NotImplementedException();
    }
}