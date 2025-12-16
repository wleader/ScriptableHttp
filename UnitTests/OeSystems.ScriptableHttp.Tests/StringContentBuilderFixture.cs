using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class StringContentBuilderFixture
{
    private StringContentBuilder _builder = null!;
    private readonly Mock<IBodyBuilder> _bodyBuilder = new();

    private const string Body = "BODY";

    private readonly PostConfig _config = new()
    {
        BodyTemplate = new()
        {
            ContentType = "text/plain; charset=utf-16",
            Value = "Hello World",
        }
    };

    [TestInitialize]
    public void Initialize()
    {
        _bodyBuilder.Reset();

        _bodyBuilder.Setup(x => x.Build(TestData.EmptyValues, _config))
            .Returns(() => Body);

        _builder = new(_bodyBuilder.Object);
    }

    [TestMethod]
    public void Given_Config_When_Build_Then_Body()
    {
        var actual = _builder.Build(TestData.EmptyValues, _config);
        Assert.IsNotNull(actual);

        // then body is correct
        var body = actual.ReadAsStringAsync().GetAwaiter().GetResult();
        Assert.AreEqual("BODY", body);

        // then content type and encoding is set
        var contentType = actual.Headers.FirstOrDefault(h => h.Key == "Content-Type").Value.ToArray();
        Assert.AreEqual(1, contentType.Length);
        Assert.AreEqual(_config.BodyTemplate.ContentType, contentType[0]);
    }

    [TestMethod]
    public void Given_Config_When_Build_Then_ContentType()
    {
        var actual = _builder.Build(TestData.EmptyValues, _config);

        // then content type and encoding is set
        var contentType = actual.Headers.FirstOrDefault(h => h.Key == "Content-Type").Value.ToArray();
        Assert.AreEqual(1, contentType.Length);
        Assert.AreEqual(_config.BodyTemplate.ContentType, contentType[0]);
    }

    [TestMethod]
    public void Given_ConfigWithoutEncoding_When_Build_Then_ContentTypeUTF8()
    {
        _config.BodyTemplate.ContentType = "text/plain";
        var actual = _builder.Build(TestData.EmptyValues, _config);

        // then content type and encoding is set
        var contentType = actual.Headers.FirstOrDefault(h => h.Key == "Content-Type").Value.ToArray();
        Assert.AreEqual(1, contentType.Length);
        Assert.AreEqual("text/plain; charset=utf-8", contentType[0]);
    }
}