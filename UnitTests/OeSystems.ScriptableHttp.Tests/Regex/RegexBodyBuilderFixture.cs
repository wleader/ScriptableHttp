using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Regex;

namespace OeSystems.ScriptableHttp.Tests.Regex;

[TestClass]
public class RegexBodyBuilderFixture
{
    private RegexBodyBuilder _builder = null!;
    private readonly Mock<IRegexMapper> _mapper = new();

    private readonly PostRequest _config = new()
    {
        BodyTemplate = new()
        {
            ContentType = "text/plain",
            Value = "TEMPLATE"
        },
        Mappings = new()
        {
            Regex = [new()]
        }
    };

    [TestInitialize]
    public void Initialize()
    {
        _mapper.Reset();

        _mapper.Setup(x => x.Replace(TestData.EmptyValues, _config.Mappings.Regex, "TEMPLATE"))
            .Returns("BUILT");
        
        _builder = new(_mapper.Object);
    }

    [TestMethod]
    public void Given_Values_And_Config_When_Build_Then_BuildResult()
    {
        var actual = _builder.Build(TestData.EmptyValues, _config);
        Assert.AreEqual("BUILT", actual);
        _mapper.Verify(x => x.Replace(TestData.EmptyValues, _config.Mappings.Regex, "TEMPLATE"), Times.Once);
        _mapper.VerifyNoOtherCalls();
    }
}