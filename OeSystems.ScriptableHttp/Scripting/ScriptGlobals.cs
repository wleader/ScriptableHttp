using System;
using System.Linq;
using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Scripting;

public class ScriptGlobals(
    IOperationHandler operationHandler,
    ScriptableHttpConfig config,
    IScriptPersistenceStore persistence)
{
    private Result _lastResult = new(ResultCode.Error, new Values());
    
    /// <summary>
    /// Allows a script to check if the last invoked operation was successful.
    /// </summary>
    public bool LastOperationSuccessful => _lastResult.IsSuccess;

    /// <summary>
    /// Allows a script to invoke an operation defined in the configuration.
    /// </summary>
    /// <param name="operationName"></param>
    /// <exception cref="ScriptException"></exception>
    public async Task OperationAsync(string operationName)
    {
        var operationConfig = config.Operation?.FirstOrDefault(x => x.Name == operationName);
        if (operationConfig == null)
        {
            throw new ScriptException("Operation not found: " + operationName);
        }
        _lastResult = await operationHandler.Invoke(operationConfig);
    }

    /// <summary>
    /// Allows a script to store a value that is re-used between script runs.
    /// </summary>
    public void SetPersisted(string key)
    {
        if (!_lastResult.Values.TryGetValue(key, out var value))
        {
            throw new ScriptException("SetPersisted Key not found: " + key);
        }
        persistence.Set(key, value);
    }

    /// <summary>
    /// Allows a script to remove a value that is persisted between script runs.
    /// </summary>
    /// <param name="key"></param>
    public void ClearPersisted(string key) => persistence.Clear(key);
}

public class ScriptException(string? message = null, Exception? inner = null) : Exception(message, inner);