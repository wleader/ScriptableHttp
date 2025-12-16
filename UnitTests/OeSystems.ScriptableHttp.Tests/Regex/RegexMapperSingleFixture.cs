using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Regex;

namespace OeSystems.ScriptableHttp.Tests.Regex;

[TestClass]
public class RegexMapperSingleFixture
{
    private RegexMapperSingle _mapper = null!;
    private readonly Mock<IValueFormatter> _formatter = new();

    [TestInitialize]
    public void Initialize()
    {
        _formatter.Reset();

        _formatter.Setup(x => x.GetFormatted(
                It.IsAny<IReadOnlyValues>(),
                It.IsAny<string>(),
                It.IsAny<string?>()))
            .Returns(GetFormatted);
        
        _mapper = new (_formatter.Object);
    }

    private static string GetFormatted(
        IReadOnlyValues values,
        string key,
        string? format)
    {
        return values.TryGetValue(key, out var obj)
            ? obj?.ToString() ?? string.Empty
            : string.Empty;
    }
    
    [TestMethod]
    [DataRow("Replace {{something}} in the middle.", "{{something}}", "42", "Replace 42 in the middle.", DisplayName = "MatchOne")]
    [DataRow("-{{something}} {{something}}-", "{{something}}", "42", "-42 42-", DisplayName = "MatchMultiple")]
    [DataRow("-57 57-", "{{something}}", "42", "-57 57-", DisplayName = "MatchNone")]
    public void Given_InputExpressionVariable_When_Replace_Then_Expected_(string input, string expression, string variable, string expected)
    {
        var config = new RegexMap()
        {
            Key = "Key",
            Format = null,
            Required = false,
            Value = expression,
        };

        var values = new Values { { "Key", variable } };

        var actual = _mapper.Replace(values, config, input);
        
        Assert.AreEqual(expected, actual);
        
        _formatter.Verify(x => x.GetFormatted(values, config.Key, config.Format), Times.Once);
        _formatter.VerifyNoOtherCalls();
    }
}