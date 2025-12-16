using System.Threading;
using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Telemetry;

namespace OeSystems.ScriptableHttp.Scripting;

public interface IScriptedOperations : IClient;

public class ScriptedOperations(
    ILibraryPersistenceStore libraryPersistence,
    IScriptCompiler compiler,
    IOperationHandler operationHandler)
    : IScriptedOperations
{
    public async Task<Result> Invoke(ScriptableHttpConfig config, IReadOnlyValues inputValues)
    {
        using var activity = new ScriptActivity(config);

        var scriptPersistence = libraryPersistence[config.Name];
        scriptPersistence.CompiledScript ??= compiler.Compile(config);
        var globals = new ScriptGlobals(operationHandler, config, scriptPersistence);

        operationHandler.Reset(inputValues);
        await scriptPersistence.CompiledScript.RunAsync(globals, CancellationToken.None);
        
        return operationHandler.LastResult;
    }
}