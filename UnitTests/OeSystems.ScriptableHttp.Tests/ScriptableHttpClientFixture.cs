using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Operations;
using OeSystems.ScriptableHttp.Scripting;
using OeSystems.ScriptableHttp.Tests.Fakes;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public sealed class ScriptableHttpClientFixture
{
    private ScriptableHttpClient _client = null!;
    private readonly Mock<ISequentialOperations> _sequentialOperations = new();
    private readonly Mock<IScriptedOperations> _scriptedOperations = new();

    private ScriptableHttpConfig _config = null!;
    private readonly ValuesResult _sequentialOperationResult = Result.FromValue(Values.Empty);
    private readonly ValuesResult _scriptedOperationResult = Result.FromValue(Values.Empty);

    [TestInitialize]
    public void Initialize()
    {
        _sequentialOperations.Reset();
        _scriptedOperations.Reset();

        _config = new()
        {
            Name = "TestScriptName"
        };

        _sequentialOperations.Setup(x => x.Invoke(_config, TestData.EmptyValues))
            .ReturnsAsync(() => _sequentialOperationResult);
        _scriptedOperations.Setup(x => x.Invoke(_config, TestData.EmptyValues))
            .ReturnsAsync(() => _scriptedOperationResult);

        _client = new (
            _sequentialOperations.Object,
            _scriptedOperations.Object);
    }

    [TestMethod]
    public async Task Given_Script_When_Invoke_Then_ScriptedOperation()
    {
        _config.Script = "Operation(\"Name\");";
        
        var actual = await _client.Invoke(_config, TestData.EmptyValues);
        
        Assert.AreEqual(_scriptedOperationResult, actual);
        _scriptedOperations.Verify(x => x.Invoke(_config, TestData.EmptyValues), Times.Once);
        
        _scriptedOperations.VerifyNoOtherCalls();
        _sequentialOperations.VerifyNoOtherCalls();
    }

    [TestMethod]
    [DataRow((string)null!)]
    [DataRow("")]
    [DataRow("\r\n\t")]
    public async Task Given_NonScript_When_Invoke_Then_SequentialOperation(string? script)
    {
        _config.Script = script;
        
        var actual = await _client.Invoke(_config, TestData.EmptyValues);
        
        Assert.AreEqual(_sequentialOperationResult, actual);
        _sequentialOperations.Verify(x => x.Invoke(_config, TestData.EmptyValues), Times.Once);
        
        _scriptedOperations.VerifyNoOtherCalls();
        _sequentialOperations.VerifyNoOtherCalls();
    }
    
    [TestMethod]
    [DataRow("Operation(\"Name\");", DisplayName = "ScriptedOperation")]
    [DataRow("", DisplayName = "SequentialOperation")]
    public async Task Given_Script_When_Invoke_Then_TraceActivity(string? script)
    {
        _config.Script = script;
        using var listener = new TestActivityListener(Telemetry.ActivitySources.Default);
        await _client.Invoke(_config, TestData.EmptyValues);
        var actual = listener.ExpectOneCompleteActivity();
        Assert.AreEqual("invoke " + _config.Name, actual.OperationName);
    }
}