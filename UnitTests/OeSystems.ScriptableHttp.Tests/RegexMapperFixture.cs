using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class RegexMapperFixture
{
    private RegexMapper _mapper = null!;
    private readonly Mock<IRegexMapperSingle> _mapperSingle = new();

    [TestInitialize]
    public void Initialize()
    {
        _mapperSingle.Reset();

        _mapperSingle.Setup(x => x.Replace(
            It.IsAny<IReadOnlyValues>(),
            It.IsAny<RegexMapping>(),
            It.IsAny<string>()))
            .Returns(() => "Result_" + _mapperSingle.Invocations.Count);
        
        _mapper = new(_mapperSingle.Object);
    }
    
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(10)]
    public void Given_Mappings_When_Replace_Then_EachMappingIsUsed(int mappingCount)
    {
        var mappings = Enumerable
            .Range(0, mappingCount)
            .Select(_ => new RegexMapping())
            .ToList();
        
        var actual = _mapper.Replace(TestData.EmptyValues, mappings, "Result_0");
        
        var expected = "Result_" + mappingCount; 
        Assert.AreEqual(expected, actual);

        for(var i =  0; i < mappingCount; i++)
        {
            var mapping = mappings[i];
            var expectedInput = "Result_" + i;
            _mapperSingle.Verify(x => x.Replace(TestData.EmptyValues, mapping, expectedInput), Times.Once);
        }
        _mapperSingle.VerifyNoOtherCalls();
    }
}