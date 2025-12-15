using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Telemetry;

namespace OeSystems.ScriptableHttp;

public interface IScriptableHttpClient : IClient;

public class ScriptableHttpClient(
    ISequentialOperations sequentialOperations,
    IScriptedOperations scriptedOperations) : IScriptableHttpClient
{
    public async Task<Result> Invoke(
        ScriptableHttpConfig config,
        IReadOnlyValues inputValues)
    {
        using var _ = new ClientInvokeActivity(config);
        return await (string.IsNullOrWhiteSpace(config.Script)
            ? sequentialOperations.Invoke(config, inputValues)
            : scriptedOperations.Invoke(config, inputValues));
    }
}