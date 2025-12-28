using System;
using Microsoft.Extensions.DependencyInjection;
using OeSystems.ScriptableHttp.Json;
using OeSystems.ScriptableHttp.Operations;
using OeSystems.ScriptableHttp.Regex;
using OeSystems.ScriptableHttp.Request;
using OeSystems.ScriptableHttp.Response;
using OeSystems.ScriptableHttp.Scripting;
using OeSystems.ScriptableHttp.Xml;
using UriBuilder = OeSystems.ScriptableHttp.Request.UriBuilder;

namespace OeSystems.ScriptableHttp;

public static class Registration
{
    public static void AddScriptableHttp(this IServiceCollection services)
    {
        services.AddScoped<IScriptableHttpClient, ScriptableHttpClient>();
        services.AddScoped<IOperationHandler, OperationHandler>();
        services.AddTransient<IValueSet, ValueSet>();
        services.AddScoped<IRequestBuilder, RequestBuilder>();
        services.AddScoped<IPostRequestBuilder, PostRequestBuilder>();
        services.AddScoped<IUriBuilder, UriBuilder>();
        services.AddScoped<IRegexMapper, RegexMapper>();
        services.AddScoped<IRegexMapperSingle, RegexMapperSingle>();
        services.AddScoped<IValueFormatter, ValueFormatter>();
        services.AddScoped<IStringContentBuilder, StringContentBuilder>();
        services.AddScoped<IBodyBuilder, BodyBuilder>();
        services.AddScoped<IRequestHeadersBuilder, RequestHeadersBuilder>();
        services.AddScoped<ICultureContext, CultureContext>();
        services.AddScoped<IGetRequestBuilder, GetRequestBuilder>();
        services.AddScoped<IResponseReader, ResponseReader>();
        services.AddScoped<ITimeoutTokenFactory, TimeoutTokenFactory>();
        services.AddScoped<IProvideOperationTimeout, ProviderOperationTimeout>();
        services.AddScoped<IRegexBodyBuilder, RegexBodyBuilder>();
        services.AddScoped<IJsonBodyBuilder, JsonBodyBuilder>();
        services.AddScoped<IXmlBodyBuilder, XmlBodyBuilder>();
        services.AddScoped<IJsonMappingHandler, JsonMappingHandler>();
        services.AddScoped<ISequentialOperations, SequentialOperations>();
        services.AddScoped<IScriptedOperations, ScriptedOperations>();
        services.AddSingleton<ILibraryPersistenceStore, LibraryPersistenceStore>();
        services.AddSingleton<TimeProvider>(_ => TimeProvider.System);
        services.AddSingleton<IScriptCompiler, ScriptCompiler>();
        services.AddScoped<IJsonResponseReader, JsonResponseReader>();
        services.AddScoped<IJsonResponseReadSingle, JsonResponseReadSingle>();
        services.AddScoped<IJsonTokenConverter, JsonTokenConverter>();
        services.AddScoped<IXmlResponseReader, XmlResponseReader>();
        services.AddScoped<IXmlReadSingle, XmlReadSingle>();
        services.AddSingleton<IXmlDocumentReader, XmlDocumentReader>();
        services.AddScoped<IXmlNodeConverter, XmlNodeConverter>();
        services.AddScoped<IRegexResponseReader, RegexResponseReader>();
        services.AddScoped<IResponseHeadersReader, ResponseHeadersReader>();
        services.AddScoped<IValueParser, ValueParser>();
    }
}