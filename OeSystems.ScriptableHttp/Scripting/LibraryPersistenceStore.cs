using System;
using System.Collections.Concurrent;
using System.Linq;
using OeSystems.ScriptableHttp.Scripting;

namespace OeSystems.ScriptableHttp;

public interface ILibraryPersistenceStore
{
    IScriptPersistenceStore this[string name] { get; }
    void Expire(TimeSpan age);
}

public class LibraryPersistenceStore(
    TimeProvider timeProvider)
    : ILibraryPersistenceStore
{
    private class ScriptData
    {
        public required IScriptPersistenceStore PersistedData { get; init; }
        public required DateTimeOffset LastUsed { get; set; }
        public required string Name { get; init; }
    }
    
    private readonly ConcurrentDictionary<string, ScriptData> _dictionary = [];

    public IScriptPersistenceStore this[string name]
    {
        get
        {
            var data = _dictionary.GetOrAdd(name, n => new()
            {
                PersistedData = new ScriptPersistenceStore(),
                LastUsed = timeProvider.GetUtcNow(),
                Name = n,
            });
            data.LastUsed = timeProvider.GetUtcNow();
            return data.PersistedData;
        }
    }

    public void Expire(TimeSpan age)
    {
        var entries = _dictionary.Values.ToList();
        var minAge = timeProvider.GetUtcNow() - age;
        foreach (var entry in entries.Where(entry => entry.LastUsed < minAge))
        {
            _dictionary.TryRemove(entry.Name, out _);
        }
    }
}