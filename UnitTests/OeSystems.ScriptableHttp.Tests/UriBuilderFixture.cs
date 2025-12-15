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
    private readonly Queue<string> _replaceResults = [];

    [TestInitialize]
    public void Initialize()
    {
        _mapper.Reset();

        _mapper.Setup(x => x.Replace(
                It.IsAny<IReadOnlyValues>(),
                It.IsAny<RegexMapping>(),
                It.IsAny<string>()))
            .Returns(() => _replaceResults.TryDequeue(out var result) ? result : null!);

        _builder = new(_mapper.Object);
    }

    [TestMethod]
    public void Given_Values_And_Config_When_Build_Then_Result()
    {
        var config = new Uri()
        {
            Value = "http://localhost/initial",
            Regex =
            [
                new(),
                new(),
            ],
        };

        _replaceResults.Clear();
        _replaceResults.Enqueue("http://localhost/replaced1");
        _replaceResults.Enqueue("http://localhost/replaced2");

        var actual = _builder.Build(TestData.EmptyValues, config);

        var expected = new System.Uri("http://localhost/replaced2");

        Assert.AreEqual(expected, actual);

        _mapper.Verify(x => x.Replace(TestData.EmptyValues, config.Regex[0], config.Value), Times.Once);
        _mapper.Verify(x => x.Replace(TestData.EmptyValues, config.Regex[1], "http://localhost/replaced1"), Times.Once);
        _mapper.VerifyNoOtherCalls();
    }
}