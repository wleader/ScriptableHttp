using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Json;
using OeSystems.ScriptableHttp.Regex;
using OeSystems.ScriptableHttp.Request;
using OeSystems.ScriptableHttp.Xml;

namespace OeSystems.ScriptableHttp.Tests.Request;

[TestClass]
public class BodyBuilderFixture
{
    private BodyBuilder _builder = null!;
    private readonly Mock<IRegexBodyBuilder> _regexBuilder = new();
    private readonly Mock<IJsonBodyBuilder> _jsonBuilder = new();
    private readonly Mock<IXmlBodyBuilder> _xmlBuilder = new();

    private PostConfig _request = null!;
    private string _buildResult = null!;

    [TestInitialize]
    public void Initialize()
    {
        _regexBuilder.Reset();
        _jsonBuilder.Reset();
        _xmlBuilder.Reset();

        _regexBuilder.Setup(x => x.Build(
                It.IsAny<IReadOnlyValues>(),
                It.IsAny<PostConfig>()))
            .Returns("REGEX_CONTENT");
        
        _jsonBuilder.Setup(x => x.Build(
            It.IsAny<IReadOnlyValues>(),
            It.IsAny<PostConfig>()))
            .Returns("JSON_CONTENT");

        _xmlBuilder.Setup(x => x.Build(
                It.IsAny<IReadOnlyValues>(),
                It.IsAny<PostConfig>()))
            .Returns("XML_CONTENT");
        
        _builder = new(
            _regexBuilder.Object,
            _jsonBuilder.Object,
            _xmlBuilder.Object);
    }

    [TestMethod]
    public void Given_RequestWithNoMappings_When_Build_ResultIsTemplate()
    {
        Given_PostRequest();
        When_Build();
        Assert.AreEqual(_request.BodyTemplate.Value, _buildResult);
    }

    [TestMethod]
    public void Given_RequestWithRegexMappings_When_Build_ResultIsRegexResult()
    {
        Given_PostRequest();
        _request.Mappings.Regex = [new()];
        When_Build();
        Assert.AreEqual("REGEX_CONTENT", _buildResult);
    }

    [TestMethod]
    public void Given_RequestWithJsonMappings_When_Build_ResultIsJsonResult()
    {
        Given_PostRequest();
        _request.Mappings.JsonPath = [new()];
        When_Build();
        Assert.AreEqual("JSON_CONTENT", _buildResult);
    }

    [TestMethod]
    public void Given_RequestWithXmlMappings_When_Build_ResultIsXmlResult()
    {
        Given_PostRequest();
        _request.Mappings.XPath = [new()];
        When_Build();
        Assert.AreEqual("XML_CONTENT", _buildResult);
    }

    private void Given_PostRequest() =>
        _request = new()
        {
            BodyTemplate = new() { Value = "TEMPLATE" },
            Mappings = new()
        };
    
    private void When_Build()
    {
        _buildResult = _builder.Build(TestData.EmptyValues, _request);
        Assert.IsNotNull(_buildResult);
    }
}