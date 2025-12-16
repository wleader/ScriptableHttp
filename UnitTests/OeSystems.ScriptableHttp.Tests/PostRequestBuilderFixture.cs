using System.Globalization;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class PostRequestBuilderFixture
{
    private PostRequestBuilder _builder = null!;
    private readonly Mock<IUriBuilder> _uriBuilder = new();
    private readonly Mock<IStringContentBuilder> _stringContentBuilder = new();
    private readonly Mock<ICultureContext> _cultureProvider = new();
    private readonly Mock<IRequestHeadersBuilder> _requestHeadersBuilder = new();

    private PostConfig _config = null!;

    private readonly Values _values = new() { { "Key", "Variable" } };

    private readonly System.Uri _uriBuilderResult = new("https://localhost/api");
    private readonly CultureInfo _cultureProviderResult = CultureInfo.GetCultureInfo("en-US");
    private readonly StringContent _contentBuilderResult = new("TEMPLATE");

    [TestInitialize]
    public void Initialize()
    {
        _uriBuilder.Reset();
        _stringContentBuilder.Reset();
        _cultureProvider.Reset();
        _requestHeadersBuilder.Reset();

        Given_Config();

        _cultureProvider.SetupGet(x => x.Current)
            .Returns(() => _cultureProviderResult);

        _uriBuilder.Setup(x => x.Build(_values, _config.Uri))
            .Returns(() => _uriBuilderResult);

        _stringContentBuilder.Setup(x => x.Build(_values, _config))
            .Returns(() => _contentBuilderResult);

        _builder = new(_uriBuilder.Object,
            _stringContentBuilder.Object,
            _cultureProvider.Object,
            _requestHeadersBuilder.Object);
    }

    [TestMethod]
    public void Given_Config_When_Build_Then_UriIsSet()
    {
        var actual = When_Build();
        _uriBuilder.Verify(x => x.Build(_values, _config.Uri), Times.Once);
        Assert.AreEqual(_uriBuilderResult, actual.RequestUri);
    }

    [TestMethod]
    public void Given_Config_When_Build_Then_ContentIsSet()
    {
        var actual = When_Build();
        Assert.AreSame(_contentBuilderResult, actual.Content);
    }

    [TestMethod]
    public void Given_Config_When_Build_Then_MethodIsSet()
    {
        var actual = When_Build();
        Assert.AreEqual(HttpMethod.Post, actual.Method);
    }

    [TestMethod]
    public void Given_Config_When_Build_Then_HeadersAreBuilt()
    {
        var actual = When_Build();
        _requestHeadersBuilder.Verify(x => x.Build(actual.Headers, _values, _config.Header), Times.Once);
    }

    [TestMethod]
    public void Given_ConfigWithoutHeaders_When_Build_Then_HeadersAreNotBuilt()
    {
        _config.Header = [];
        var actual = When_Build();
        _requestHeadersBuilder.Verify(x => x.Build(actual.Headers, _values, _config.Header), Times.Never);
    }

    private void Given_Config()
    {
        _config = new()
        {
            Uri = new()
            {
                Value = "https://localhost/api"
            },
            BodyTemplate = new()
            {
                ContentType = "text/plain; charset=utf-8",
                Value = "TEMPLATE",
            },
            Culture = "en-US",
            Header = [new() { Name = "CustomHeader", Value = "CustomValue" }]
        };
    }

    private HttpRequestMessage When_Build()
    {
        var result = _builder.Build(_values, _config);
        Assert.IsNotNull(result);
        return result;
    }
}