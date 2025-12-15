using System.Net.Http;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IPostRequestBuilder
{
    HttpRequestMessage Build(IReadOnlyValues values, PostRequest config);
}

public class PostRequestBuilder(
    IUriBuilder uriBuilder,
    IStringContentBuilder stringContentBuilder,
    ICultureContext cultureContext,
    IRequestHeadersBuilder requestHeadersBuilder)
    : IPostRequestBuilder
{
    public HttpRequestMessage Build(IReadOnlyValues values, PostRequest config)
    {
        using var _ = cultureContext.Set(config.Culture);

        var result = new HttpRequestMessage
        {
            RequestUri = uriBuilder.Build(values, config.Uri),
            Content = stringContentBuilder.Build(values, config),
            Method = HttpMethod.Post,
        };

        if (config.HeaderSpecified)
            requestHeadersBuilder.Build(result.Headers, values, config.Header);

        return result;
    }
}