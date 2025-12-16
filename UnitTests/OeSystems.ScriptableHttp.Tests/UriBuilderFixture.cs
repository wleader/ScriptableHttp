using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class UriBuilderFixture
{
    private UriBuilder _builder = null!;
    private readonly Mock<IRegexMapper> _mapper = new();

    [TestInitialize]
    public void Initialize()
    {
        _mapper.Reset();

        _mapper.Setup(x => x.Replace(
                It.IsAny<IReadOnlyValues>(),
                It.IsAny<IEnumerable<RegexMapping>>(),
                It.IsAny<string>()))
            .Returns(() => "http://localhost/replaced");

        _builder = new(_mapper.Object);
    }

    [TestMethod]
    public void Given_Values_And_Config_When_Build_Then_Result()
    {
        var config = new Uri
        {
            Value = "http://localhost/initial",
            Regex = [new()],
        };

        var actual = _builder.Build(TestData.EmptyValues, config);

        var expected = new System.Uri("http://localhost/replaced");

        Assert.AreEqual(expected, actual);

        _mapper.Verify(x => x.Replace(TestData.EmptyValues, config.Regex, config.Value), Times.Once);
        _mapper.VerifyNoOtherCalls();
    }
}