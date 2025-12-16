using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class JsonBodyBuilderFixture
{
    private JsonBodyBuilder _builder = null!;
    private readonly Mock<IJsonMappingHandler> _handler = new();

    private readonly PostConfig _configuration = new()
    {
        BodyTemplate = new()
        {
            Value = "{}",
            ContentType = "application/json"
        },
        Mappings = new() {JsonPath = [new (), new ()]}
    };
    [TestInitialize]
    public void Initialize()
    {
        _handler.Reset();
        _builder = new(_handler.Object);
    }

    [TestMethod]
    public void Given_Values_And_Config_When_Build_Then_Result()
    {
        var result = _builder.Build(TestData.EmptyValues, _configuration);
        Assert.AreEqual("{}", result);
    }
    
    [TestMethod]
    public void Given_Values_And_Config_When_Build_Then_MappingsInvoked()
    {
        _ = _builder.Build(TestData.EmptyValues, _configuration);
        _handler.Verify(x => x.PerformMap(
                TestData.EmptyValues,
                It.IsAny<JObject>(),
                It.IsAny<JsonPathMap>()),
            Times.Exactly(_configuration.Mappings.JsonPath.Count));
        _handler.VerifyNoOtherCalls();
        var args = _handler.Invocations.Select(x => x.Arguments[2] as JsonPathMap).ToList();
        CollectionAssert.AreEqual(_configuration.Mappings.JsonPath, args);
    }

    [TestMethod]
    public void Given_HandlerMutatesDocument_When_Build_Then_Result()
    {
        _handler.Setup(x => x.PerformMap(TestData.EmptyValues, It.IsAny<JObject>(), It.IsAny<JsonPathMap>()))
            .Callback((IReadOnlyValues _, JObject o, JsonPathMap _) =>
            {
                o.Add("Key" + o.Count, JToken.FromObject("Value" + o.Count));
            });

        const string expected = 
            """
            {
              "Key0": "Value0",
              "Key1": "Value1"
            }
            """;
        
        var result = _builder.Build(TestData.EmptyValues, _configuration);
        Assert.AreEqual(expected, result);
    }
}