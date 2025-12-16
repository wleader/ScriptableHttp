using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Scripting;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class SequentialOperationsFixture
{
    private SequentialOperations _client = null!;
    private readonly Mock<IOperationHandler> _operationHandler = new();
    private readonly Values _outputValues = TestData.CreateEmptyValues();

    [TestInitialize]
    public void Initialize()
    {
        _operationHandler.Reset();

        _operationHandler.Setup(x => x.Invoke(
                It.IsAny<OperationConfig>()))
            .Callback((OperationConfig _) => _operationHandler.Verify(
                x => x.Reset(It.IsAny<IReadOnlyValues>()),
                Times.Once,
                "The Operation Handler must be reset before any invocations."))
            .ReturnsAsync(() => new (ResultCode.Success, _outputValues));

        _client = new (
            _operationHandler.Object);
    }

    [TestMethod]
    public async Task Given_InputValues_And_Configuration_When_Invoke_Then_Result_Is_Success()
    {
        var inputValues = Given_InputValues();
        var configuration = Given_Configuration();
        var actual = await _client.Invoke(configuration, inputValues);
        Assert.IsTrue(actual.IsSuccess);
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(10)]
    public async Task Given_InputValues_And_ConfigurationWithMultipleOperations_When_Invoke_Then_MultipleOperationsArePerformedInOrder(
        int operationCount)
    {
        var inputValues = Given_InputValues();

        var configuration = new ScriptableHttpConfig
        {
            Name = "Multiple Operations Configuration"
        };
        for (var x = 0; x < operationCount; x++)
        {
            configuration.Operation.Add(new()
            {
                Name = "Operation " + x,
            });
        }

        var actual = await _client.Invoke(configuration, inputValues);
        Assert.IsTrue(actual.IsSuccess);

        _operationHandler.Verify(x => x.Invoke(
                It.IsAny<OperationConfig>()
            ),
            Times.Exactly(configuration.Operation.Count));

        var invocationOrder = _operationHandler.Invocations
            .Where(x => x.Method.Name == nameof(IOperationHandler.Invoke))
            .Select(x => x.Arguments[0] as OperationConfig).ToList();
        CollectionAssert.AreEqual(configuration.Operation, invocationOrder);
    }

    [TestMethod]
    public async Task Given_InputValues_And_Configuration_And_Operation1ReturnsError_When_Invoke_Then_Result_Is_Error()
    {
        var inputValues = Given_InputValues();
        var configuration = Given_Configuration();

        _operationHandler.Setup(x => x.Invoke(configuration.Operation[0]))
            .ReturnsAsync(new Result(ResultCode.Error, _outputValues));

        var actual = await _client.Invoke(configuration, inputValues);
        Assert.IsTrue(actual.IsError);
        Assert.AreSame(actual.Values, _outputValues);
    }

    [TestMethod]
    public async Task Given_InputValues_And_Configuration_And_Operation2ReturnsError_When_Invoke_Then_Result_Is_Error()
    {
        var inputValues = Given_InputValues();
        var configuration = Given_Configuration();

        _operationHandler.Setup(x => x.Invoke(configuration.Operation[1]))
            .ReturnsAsync(new Result(ResultCode.Error, _outputValues));

        var actual = await _client.Invoke(configuration, inputValues);
        Assert.IsTrue(actual.IsError);
        Assert.AreSame(actual.Values, _outputValues);
        _operationHandler.Verify(x => x.Invoke(configuration.Operation[0]), Times.Once);
        _operationHandler.Verify(x => x.Invoke(configuration.Operation[1]), Times.Once);
        _operationHandler.VerifyNoOtherCalls();
    }

    private static Values Given_InputValues()
    {
        var result = new Values
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };
        return result;
    }

    private static ScriptableHttpConfig Given_Configuration() => new()
    {
        Name = "Configuration Name",
        Operation =
        [
            new() { Name = "Operation 1" },
            new() { Name = "Operation 2" },
        ]
    };
}