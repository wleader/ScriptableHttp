using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class GetRequestBuilderFixture
{
    private GetRequestBuilder _builder = null!;
    private readonly Mock<IUriBuilder> _uriBuilder = new();
    private readonly Mock<IRequestHeadersBuilder> _headersBuilder = new();
    private readonly Mock<ICultureContext> _cultureProvider = new();

    private readonly System.Uri _builtUri = new("http://localhost");
    private readonly CultureInfo _culture = new("en-US");

    private readonly GetRequest _config = new()
    {
        Uri = new(),
    };

    [TestInitialize]
    public void Initialize()
    {
        _uriBuilder.Reset();
        _headersBuilder.Reset();
        
        _cultureProvider.SetupGet(x => x.Current)
            .Returns(() => _culture);

        _uriBuilder.Setup(x => x.Build(TestData.EmptyValues, _config.Uri))
            .Returns(() => _builtUri);

        _builder = new(
            _uriBuilder.Object,
            _headersBuilder.Object,
            _cultureProvider.Object);
    }

    [TestMethod]
    public void Given_Values_And_Config_When_Build_Then_UriIsBuilt()
    {
        var actual = _builder.Build(TestData.EmptyValues, _config);
        Assert.AreSame(_builtUri, actual.RequestUri);
    }

    [TestMethod]
    public void Given_Values_And_Config_When_Build_Then_MethodIsSet()
    {
        var actual = _builder.Build(TestData.EmptyValues, _config);
        Assert.AreEqual("GET", actual.Method.Method);
    }

    [TestMethod]
    public void Given_Value_And_ConfigWithHeaders_When_Build_Then_HeadersAreBuilt()
    {
        _config.Header = [new()];
        var actual = _builder.Build(TestData.EmptyValues, _config);
        _headersBuilder.Verify(x => x.Build(actual.Headers, TestData.EmptyValues, _config.Header), Times.Once);
    }

    [TestMethod]
    public void Given_Values_And_ConfigWithoutHeaders_When_Build_Then_HeadersAreNotBuilt()
    {
        _config.Header.Clear();
        _ = _builder.Build(TestData.EmptyValues, _config);
        _headersBuilder.Verify(x => x.Build(
                It.IsAny<HttpRequestHeaders>(),
                It.IsAny<IReadOnlyValues>(),
                It.IsAny<IList<RequestHeader>>()),
            Times.Never);
    }
}