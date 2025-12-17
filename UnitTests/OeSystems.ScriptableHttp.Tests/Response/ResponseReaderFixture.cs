using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Json;
using OeSystems.ScriptableHttp.Regex;
using OeSystems.ScriptableHttp.Response;
using OeSystems.ScriptableHttp.Xml;

namespace OeSystems.ScriptableHttp.Tests.Response;

[TestClass]
public class ResponseReaderFixture
{
    private ResponseReader _reader = null!;
    private readonly Mock<IJsonResponseReader> _jsonReader = new();
    private readonly Mock<IRegexResponseReader> _regexReader = new();
    private readonly Mock<IXmlResponseReader> _xmlReader = new();
    private readonly Mock<IResponseHeadersReader> _headersReader = new();

    private HttpResponseMessage _response = null!;
    private ResponseConfig _config = null!;
    private IReadOnlyValues _actual = null!;

    [TestInitialize]
    public void Initialize()
    {
        _jsonReader.Reset();
        _regexReader.Reset();
        _xmlReader.Reset();
        _headersReader.Reset();

        _response = new()
        {
            Content = new StringContent("RESPONSE_CONTENT"),
            StatusCode = HttpStatusCode.OK,
        };
        _response.Headers.Add("Name1", "Value1");

        _config = new() { Mappings = new() };
        
        _jsonReader.Setup(x => x.Read(
                It.IsAny<string>(),
                It.IsAny<IEnumerable<JsonPathMap>>(),
                It.IsAny<Values>()))
            .Callback((string _, IEnumerable<JsonPathMap> _, Values v) =>
                v.Add("JsonKey", "JsonValue"));
        
        _regexReader.Setup(x => x.Read(
                It.IsAny<string>(),
                It.IsAny<IEnumerable<RegexMap>>(),
                It.IsAny<Values>()))
            .Callback((string _, IEnumerable<RegexMap> _, Values v) =>
                v.Add("RegexKey", "RegexValue"));

        _xmlReader.Setup(x => x.Read(
                It.IsAny<string>(),
                It.IsAny<IEnumerable<XPathMap>>(),
                It.IsAny<Values>()))
            .Callback((string _, IEnumerable<XPathMap> _, Values v) =>
                v.Add("XmlKey", "XmlValue"));

        _headersReader.Setup(x => x.Read(
                It.IsAny<HttpResponseHeaders>(),
                It.IsAny<IEnumerable<ResponseHeaderConfig>>(),
                It.IsAny<Values>()))
            .Callback((HttpResponseHeaders _, IEnumerable<ResponseHeaderConfig> _, Values v) =>
                v.Add("HeadersKey", "HeadersValue"));
        
        _reader = new(
            _jsonReader.Object,
            _regexReader.Object,
            _xmlReader.Object,
            _headersReader.Object);
    }

    [TestMethod]
    public async Task Given_EmptyMappings_When_Read_Then_ReadersInvoked_And_ResultContainsReaderResults()
    {
        _config.Header = [];
        _config.Mappings = new()
        {
            Regex = [],
            JsonPath = [],
            XPath = [],
        };
        await When_Read();
        Then_ResultContainsReaderResults();
        Then_ReadersWereInvoked();
    }

    [TestMethod]
    public async Task Given_NullMappings_When_Read_Then_ReadersInvoked_And_ResultContainsReaderResults()
    {
        _config.Header = null;
        _config.Mappings = new()
        {
            Regex = null,
            JsonPath = null,
            XPath = null,
        };
        await When_Read();
        Then_ResultContainsReaderResults();
        Then_ReadersWereInvoked();
    }
    
    [TestMethod]
    public async Task Given_NullMappingsRoot_When_Read_Then_ReadersInvoked_And_ResultContainsReaderResults()
    {
        _config.Mappings = null;
        await When_Read();
        Then_ResultContainsReaderResults();
        Then_ReadersWereInvoked();
    }

    [TestMethod]
    public async Task Given_Mappings_When_Read_Then_ReadersInvoked_And_ResultContainsReaderResults()
    {
        _config.Header = [new()];
        _config.Mappings.JsonPath = [new()];
        _config.Mappings.Regex = [new()];
        _config.Mappings.XPath = [new()];
        await When_Read();
        Then_ResultContainsReaderResults();
        Then_ReadersWereInvoked();
    }

    private async Task When_Read() => _actual = await _reader.Read(_response, _config);

    private void Then_ResultContainsReaderResults()
    {
        Assert.AreEqual("JsonValue", _actual["JsonKey"]);
        Assert.AreEqual("RegexValue", _actual["RegexKey"]);
        Assert.AreEqual("XmlValue", _actual["XmlKey"]);
        Assert.AreEqual("HeadersValue", _actual["HeadersKey"]);
        Assert.AreEqual(4, _actual.Count);
    }
    
    private void Then_ReadersWereInvoked()
    {
        var expectedJsonMaps = _config.Mappings?.JsonPath ?? [];
        _jsonReader.Verify(x => x.Read("RESPONSE_CONTENT", expectedJsonMaps, It.IsAny<Values>()), Times.Once);
        _jsonReader.VerifyNoOtherCalls();
        
        var expectedXmlMaps = _config.Mappings?.XPath ?? [];
        _xmlReader.Verify(x => x.Read("RESPONSE_CONTENT", expectedXmlMaps, It.IsAny<Values>()), Times.Once);
        _xmlReader.VerifyNoOtherCalls();
        
        var expectedRegexMaps = _config.Mappings?.Regex ?? [];
        _regexReader.Verify(x => x.Read("RESPONSE_CONTENT", expectedRegexMaps, It.IsAny<Values>()), Times.Once);
        _regexReader.VerifyNoOtherCalls();
        
        var expectedHeadersConfig = _config.Header ?? [];
        _headersReader.Verify(x => x.Read(_response.Headers, expectedHeadersConfig, It.IsAny<Values>()), Times.Once);
        _headersReader.VerifyNoOtherCalls();
    }
}