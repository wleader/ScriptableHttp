using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Regex;
using OeSystems.ScriptableHttp.Request;

namespace OeSystems.ScriptableHttp.Tests.Request;

[TestClass]
public class RequestHeadersBuilderFixture
{
    private RequestHeadersBuilder _builder = null!;
    private readonly Mock<IRegexMapper> _mapper = new();

    [TestInitialize]
    public void Initialize()
    {
        _mapper.Reset();

        _mapper.Setup(x => x.Replace(
                It.IsAny<IReadOnlyValues>(),
                It.IsAny<IEnumerable<RegexMap>>(),
                It.IsAny<string>()))
            .Returns((IReadOnlyValues _, IEnumerable<RegexMap> _, string i) => i);

        _builder = new(_mapper.Object);
    }
    
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(10)]
    public void Given_HeaderConfigurations_When_Build_Then_HeadersAdded(int headerCount)
    {
        var config = Enumerable.Range(1, headerCount)
            .Select(x => new RequestHeaderConfig()
            {
                Name = $"Name_{x}",
                Regex = [new()],
                Value = $"Value_{x}",
            })
            .ToList();
        
        var request = new HttpRequestMessage();
        _builder.Build(request.Headers, TestData.EmptyValues, config);

        foreach (var headerConfig in config)
        {
            var match = request.Headers.Single(x => x.Key == headerConfig.Name).Value.Single();
            Assert.AreEqual(headerConfig.Value, match);
            _mapper.Verify(x => x.Replace(TestData.EmptyValues, headerConfig.Regex, headerConfig.Value), Times.Once);
        }
        _mapper.VerifyNoOtherCalls();
    }
}