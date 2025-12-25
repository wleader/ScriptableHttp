using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Tests.Fakes;
using OeSystems.ScriptableHttp.Tests.Resources;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class TemporaryIntegrationTests
{
    private ServiceCollection _serviceCollection = null!;
    private readonly FakeHttpClientFactory _httpClientFactory = new();
    private AsyncServiceScope _scope;
    private IScriptableHttpClient _client = null!;
    private ServiceProvider _serviceProvider = null!;
    
    [TestInitialize]
    public void Initialize()
    {
        _httpClientFactory.Reset();
        _serviceCollection = [];
        _serviceCollection.AddSingleton<IHttpClientFactory>(_ => _httpClientFactory);
        _serviceCollection.AddScriptableHttp();
        _serviceProvider = _serviceCollection.BuildServiceProvider();
        _scope = _serviceProvider.CreateAsyncScope();
        _client = _scope.ServiceProvider.GetRequiredService<IScriptableHttpClient>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _scope.Dispose();
    }

    private void SetupResponse(Uri? uri, HttpResponseMessage response)
    {
        _httpClientFactory.Handler.Mock.Setup(x => x.SendAsync(It.Is<HttpRequestMessage>(m => m.RequestUri == uri),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
    }

    private static HttpResponseMessage BuildResponse(Stream response, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ()
        {
            Content = new StreamContent(response),
            StatusCode = statusCode,
        };
    }

    [TestMethod]
    [Ignore] // don't use this for code coverage.
    public async Task When_Invoke()
    {
        var config = TestResources.LibraryIntegration1();

        var values = new Values
        {
            { "Key1", "Value1" },
        };
        
        SetupResponse(
            new("http://localhost/operation1"),
            BuildResponse(TestResources.BasicGetResponseJson()));
        
        SetupResponse(
            new("http://localhost/gettoken"),
            new (HttpStatusCode.OK){Content = new StringContent(Guid.NewGuid().ToString())});
        
        var result = await _client.Invoke(config, values);
        
        Assert.IsTrue(result.IsSuccess);
        var resultValues = result.Value;

        Console.WriteLine("Result Values:");
        Console.WriteLine(JsonSerializer.Serialize(resultValues, PrettyJson));

        Assert.IsTrue(resultValues.ContainsKey("Status"));
        var actual = resultValues["Status"];
        Assert.AreEqual("OK",actual);
        
        Assert.IsTrue(resultValues.ContainsKey("StatusCode"));
        var actualStatusCode = resultValues["StatusCode"];
        Assert.AreEqual(0, actualStatusCode);
        
        Assert.IsTrue(resultValues.ContainsKey("StatusCodeStr"));
        var stringStatusCode = resultValues["StatusCodeStr"];
        Assert.AreEqual("0", stringStatusCode);
    }

    private static readonly JsonSerializerOptions PrettyJson = new()
    {
        WriteIndented = true,
    };
}