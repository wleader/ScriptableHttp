using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IClient
{
    public Task<ValuesResult> Invoke(
        ScriptableHttpConfig config,
        IReadOnlyValues inputValues);
}