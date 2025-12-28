using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Json;

namespace OeSystems.ScriptableHttp.Tests.Json;

[TestClass]
public class JsonResponseReaderFixture
{
    private JsonResponseReader _reader = null!;
    private readonly Mock<IJsonResponseReadSingle> _readSingle = new();

    private const string Body =
        """
        {
        "Name":"Value"
        }
        """;

    [TestInitialize]
    public void Initialize()
    {
        _readSingle.Reset();
        _reader = new(_readSingle.Object);
    }

    [TestMethod]
    public void Given_EmptyMappings_When_Read_Then_NothingIsRead()
    {
        Assert.IsTrue(_reader.Read(Body, [], new()).IsSuccess);
        _readSingle.Verify(x => x.Read(
            It.IsAny<JObject>(),
            It.IsAny<JsonPathMap>(),
            It.IsAny<Values>(),
            It.IsAny<Errors>()), Times.Never);
    }

    [TestMethod]
    public void Given_SingleReadUpdatesValues_When_Read_Then_ValuesAreUpdated()
    {
        _readSingle.Setup(x => x.Read(
                It.IsAny<JObject>(),
                It.IsAny<JsonPathMap>(),
                It.IsAny<Values>(),
                It.IsAny<Errors>()))
            .Callback((JObject _,  JsonPathMap _, Values v, Errors _) =>
                v.Add($"Key{v.Count}", $"Value{v.Count}"));
        var mappings = Given_Mappings(1);
        var values = new Values();
        var result = _reader.Read(Body, mappings, values);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, values.Count);
        Assert.AreEqual("Value0", values["Key0"]);
    }

    [TestMethod]
    public void Given_SingleReadUpdatesErrors_When_Read_Then_ResultIsError()
    {
        _readSingle.Setup(x => x.Read(
            It.IsAny<JObject>(),
            It.IsAny<JsonPathMap>(),
            It.IsAny<Values>(),
            It.IsAny<Errors>()))
            .Callback((JObject _,  JsonPathMap _, Values _, Errors e) => 
                e.Add(new("Test Error")));
        
        var mappings = Given_Mappings(1);
        var result = _reader.Read(Body, mappings, new());
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsNull(result.Errors[0].Exception);
        Assert.AreEqual("Test Error", result.Errors[0].Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(10)]
    public void Given_MultipleMappings_When_Read_Then_MultipleReads(int count)
    {
        var values = new Values();
        var mappings = Given_Mappings(count);
        Assert.IsTrue(_reader.Read(Body, mappings, new()).IsSuccess);

        foreach (var map in mappings)
        {
            _readSingle.Verify(x => x.Read(
                    It.IsAny<JObject>(),
                    map,
                    values,
                    It.IsAny<Errors>()),
                Times.Once);
        }

        _readSingle.VerifyNoOtherCalls();
        Assert.AreEqual(1, _readSingle.Invocations.Select(x => x.Arguments[0] as JObject).Distinct().Count());
        Assert.AreEqual(1, _readSingle.Invocations.Select(x => x.Arguments[2] as Values).Distinct().Count());
        Assert.AreEqual(1, _readSingle.Invocations.Select(x => x.Arguments[3] as Errors).Distinct().Count());
    }

    [TestMethod]
    public void Given_Body_When_Read_Then_DocumentIsFromBody()
    {
        const string unindented = "{\"Name\":\"Value\"}";
        var mappings = Given_Mappings(1);
        Assert.IsTrue(_reader.Read(unindented, mappings, new()).IsSuccess);
        var doc = _readSingle.Invocations[0].Arguments[0] as JObject;
        Assert.IsNotNull(doc);
        var actual = doc.ToString(Formatting.None);
        Assert.AreEqual(unindented, actual);
    }

    private List<JsonPathMap> Given_Mappings(int count) =>
        Enumerable.Range(0, count)
            .Select(_ => new JsonPathMap())
            .ToList();
}