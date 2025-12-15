using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.Scripting;

namespace OeSystems.ScriptableHttp;

public interface IScriptPersistenceStore
{
    void Set(string key, object? value);
    void Clear(string key);
    Script<object>? CompiledScript { get; set; }
}

public class ScriptPersistenceStore : IScriptPersistenceStore
{
    private readonly ConcurrentDictionary<string, object?> _dictionary = [];

    public void Set(string key, object? value) => 
        _dictionary.AddOrUpdate(key, value,  (_, _) => value);

    public void Clear(string key) => _dictionary.TryRemove(key, out _);

    public Script<object>? CompiledScript { get; set; }
}