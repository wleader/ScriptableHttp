using System.Collections.Generic;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Regex;

public interface IRegexMapper
{
    string Replace(IReadOnlyValues values, IEnumerable<RegexMapping> mapping, string input);
}



public class RegexMapper(IRegexMapperSingle mapperSingle) : IRegexMapper
{
    public string Replace(IReadOnlyValues values, IEnumerable<RegexMapping> mapping, string input)
    {
        var result = input;
        foreach (var m in mapping)
        {
            result = mapperSingle.Replace(values, m, result);
        }
        return result;
    }
}