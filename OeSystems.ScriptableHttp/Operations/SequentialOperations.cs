using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Operations;

public interface ISequentialOperations : IClient;

public class SequentialOperations(
    IOperationHandler operationHandler)
    : ISequentialOperations
{
    public async Task<ValuesResult> Invoke(ScriptableHttpConfig config, IReadOnlyValues inputValues)
    {
        operationHandler.Reset(inputValues);
        ValuesResult result = default;
        foreach (var operation in config.Operation)
        {
            result = await operationHandler.Invoke(operation);
            if (result.IsError)
                return result;
        }
        return result;
    }
}