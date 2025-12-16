using System;
using System.Net.Http;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Request;

public interface IRequestBuilder
{
    HttpRequestMessage Build(IReadOnlyValues values, RequestConfig config);
}

public class RequestBuilder(
    IPostRequestBuilder postBuilder,
    IGetRequestBuilder getBuilder)
    : IRequestBuilder
{
    public HttpRequestMessage Build(IReadOnlyValues values, RequestConfig config)
    {
        if (config.Post is not null)
            return postBuilder.Build(values, config.Post);

        return config.Get is not null 
            ? getBuilder.Build(values, config.Get)
            : throw new ApplicationException("A Get or Post is required in the Request configuration.");
    }
}