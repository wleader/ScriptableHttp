using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Tests.Fakes;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class JsonMappingHandlerFixture
{
    private JsonMappingHandler _handler = null!;
    private readonly FakeFormatter _formatter = new();

    private readonly Values _values = new()
    {
        { "Key1", "newValue" },
    };

    private readonly JsonPathMapping _configuration = new()
    {
        Key = "Key1",
        Query = "$.parent.child",
        Format = "FORMAT",
        Required = true,
    };

    [TestInitialize]
    public void Initialize()
    {
        _formatter.Reset();
        _handler = new(_formatter);
    }

    [TestMethod]
    public void Given_ValuesAndConfig_When_PerformMap_DocumentIsModified()
    {
        const string originalJson =
            $$"""
              {
                "parent": {
                  "child": "oldValue"
                }
              }
              """;
        var document = JObject.Parse(originalJson);
        _handler.PerformMap(_values, document, _configuration);
        var modified = document["parent"]?["child"]?.ToString();
        Assert.AreEqual("newValue", modified);
        
        _formatter.Mock.Verify(x => x.GetFormatted(_values, "Key1", "FORMAT"), Times.Once);
        _formatter.Mock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void PerformMap_WritingValuesThatAreNotStrings()
    {
        Assert.Inconclusive("Behavior for writing values that are not strings is not implemented.");
    }
    
    [TestMethod]
    public void PerformMap_PathMulitpleMatched()
    {
        Assert.Inconclusive("Behavior for when the JsonPath has multiple matches is not implemented.");
    }

    [TestMethod]
    public void PerformMap_PathNotMatched()
    {
        Assert.Inconclusive("Behavior for when the JsonPath is not matched is not implemented.");
    }

    [TestMethod]
    public void PerformMap_RegexOnJsonPath()
    {
        Assert.Inconclusive("Regex not applied to JsonPath.");
    }
}