using System.Net.Http;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Request;

public interface IGetRequestBuilder
{
    HttpRequestMessage Build(IReadOnlyValues values, GetConfig config);
}

public class GetRequestBuilder(
    IUriBuilder uriBuilder,
    IRequestHeadersBuilder headersBuilder,
    ICultureContext cultureContext)
    : IGetRequestBuilder
{
    public HttpRequestMessage Build(IReadOnlyValues values, GetConfig config)
    {
        using var _ = cultureContext.Set(config.Culture);

        var result = new HttpRequestMessage()
        {
            RequestUri = uriBuilder.Build(values, config.Uri),
            Method = HttpMethod.Get,
        };

        if (config.HeaderSpecified)
            headersBuilder.Build(result.Headers, values, config.Header);

        return result;
    }
}