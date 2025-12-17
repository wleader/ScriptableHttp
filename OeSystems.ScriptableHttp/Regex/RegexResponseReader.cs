using System.Collections.Generic;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Regex;

public interface IRegexResponseReader
{
    public void Read(string body, IEnumerable<RegexMap> mappings, Values values);
}

public class RegexResponseReader : IRegexResponseReader
{
    public void Read(string body, IEnumerable<RegexMap> mappings, Values values)
    {
        throw new System.NotImplementedException();
    }
}