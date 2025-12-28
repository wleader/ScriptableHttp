using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Json;
using OeSystems.ScriptableHttp.Regex;

namespace OeSystems.ScriptableHttp.Tests.Json;

[TestClass]
public class JsonResponseReadSingleFixture
{
    private JsonResponseReadSingle _readSingle = null!;
    private readonly Mock<IRegexMapper> _regex = new();
    private readonly Mock<IJsonTokenConverter> _converter = new();

    private JObject _document = null!;
    private Values _values = null!;
    private Errors _errors = null!;
    private JsonPathMap _map = null!;

    private const string Body =
        """
        {
          "Unique":"UniqueValue",
          "Duplicate":"DuplicateValueA",
          "Duplicate":"DuplicateValueB",
          "RegexReplaced":"RegexValue"
        }
        """;

    [TestInitialize]
    public void Initialize()
    {
        _values = new();
        _errors = new();

        _map = new()
        {
            DataType = "System.String",
            Format = null,
            Key = "Name",
            Query = "$.NotSet",
            Regex = [new()],
            Required = true
        };

        _document = JObject.Parse(Body);

        _regex.Reset();
        _converter.Reset();

        _converter.Setup(x => x.Convert(
                It.IsAny<JToken>(),
                It.Is<JsonPathMap>(m => ReferenceEquals(m, _map))))
            .Returns(() => Result<object?>.Success("ConverterResult"));
        
        _regex.Setup(x => x.Replace(
                It.IsAny<IReadOnlyValues>(),
                It.IsAny<IEnumerable<RegexMap>>(),
                It.IsAny<string>()))
            .Returns((IReadOnlyValues _, IEnumerable<RegexMap> _, string s) => s);
        
        _readSingle = new(_regex.Object, _converter.Object);
    }

    [TestMethod]
    public void Given_RequiredMappingThatMatches_When_Read_Then_ValueIsRead()
    {
        _map.Required = true;
        _map.Query = "$.Unique";
        When_Read();
        Then_NoErrors();
        Then_ValueWasRead("Unique", "ConverterResult");
    }

    [TestMethod]
    public void Given_RequiredMappingThatDoesNotMatch_When_Read_Then_Error()
    {
        _map.Required = true;
        _map.Query = "$.NotFound";
        When_Read();
        Then_NothingWasRead();
        Assert.AreEqual(1, _errors.Count);
        Assert.IsTrue(_errors[0].Message.Contains(_map.Query));
    }

    private void Then_NothingWasRead()
    {
        _converter.Verify(x => x.Convert(
            It.IsAny<JToken>(),
            _map),
            Times.Never);
        Assert.AreEqual(0, _values.Count);
    }

    [TestMethod]
    public void Given_OptionalMappingThatDoesNotMatches_When_Read_Then_NothingIsRead()
    {
        _map.Required = false;
        _map.Query = "$.NotFound";
        When_Read();
        Then_NoErrors();
        Then_NothingWasRead();
    }

    [TestMethod]
    public void Given_MappingWithMultipleMatches_When_Read_Then_LastValueIsRead()
    {
        _map.Query = "$.Duplicate";
        When_Read();
        Then_NoErrors();
        Then_ValueWasRead("Duplicate", "ConverterResult");
    }

    [TestMethod]
    public void Given_MappingWithRegex_When_Read_Then_QueryIsReplaced()
    {
        _map.Query = "$.RegexOriginal";
        _regex.Setup(x => x.Replace(
                _values,
                _map.Regex,
                _map.Query))
            .Returns("$.RegexReplaced");

        When_Read();
        Then_NoErrors();
        Then_ValueWasRead("RegexReplaced", "ConverterResult");
    }

    [TestMethod]
    public void Given_MappingSpecifiesType_When_Read_Then_ValueIsRead()
    {
        _map.DataType = "System.Int32";
        _map.Query = "$.Name";
        _document = JObject.Parse("""{"Name":42}""");
        When_Read();
        Then_ValueWasRead("Name", "ConverterResult");
    }
    
    private void When_Read()
    {
        _readSingle.Read(_document, _map, _values, _errors);
    }

    private void Then_ValueWasRead<T>(string path, T expected)
    {
        Assert.AreEqual(1, _values.Count);
        Assert.IsTrue(_values.ContainsKey(_map.Key));
        var actual = _values[_map.Key];
        Assert.IsNotNull(actual);
        Assert.AreEqual(typeof(T), actual.GetType());
        Assert.AreEqual(expected, actual);
        
        Then_TokenWasConverted(path);
    }
    
    private void Then_TokenWasConverted(string path)
    {
        _converter.Verify(x => x.Convert(
            It.Is<JToken>(t => t.Path == path),
            _map), Times.Once);
    }
    
    private void Then_NoErrors()
    {
        Assert.AreEqual(0, _errors.Count);
    }
}