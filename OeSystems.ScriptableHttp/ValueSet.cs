using System.Collections.Generic;

namespace OeSystems.ScriptableHttp;

public interface IValueSet
{
    void Reset(IReadOnlyValues initialValues);
    void Add(IReadOnlyValues values);
    IReadOnlyValues Combine();
}

public class ValueSet : IValueSet
{
    private readonly List<IReadOnlyValues> _list = [];

    public void Reset(IReadOnlyValues initialValues)
    {
        _list.Clear();
        _list.Add(initialValues);
    }

    public void Add(IReadOnlyValues values) => _list.Add(values);

    public IReadOnlyValues Combine()
    {
        var result = new Values();
        foreach (var v in _list)
        {
            result.Add(v);
        }
        return result;
    }
}