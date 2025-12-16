using System;
using Microsoft.Extensions.DependencyInjection;
using OeSystems.ScriptableHttp.Regex;
using OeSystems.ScriptableHttp.Request;
using OeSystems.ScriptableHttp.Scripting;
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
    }
}