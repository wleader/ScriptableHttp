using System.Collections.Generic;
using System.Text.RegularExpressions;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IRegexMapper
{
    string Replace(IReadOnlyValues values, IEnumerable<RegexMapping> mapping, string input);
}

public interface IRegexMapperSingle
{
    string Replace(IReadOnlyValues values, RegexMapping mapping, string input);
}

public class RegexMapperSingle(IValueFormatter valueFormatter) : IRegexMapperSingle
{
    public string Replace(IReadOnlyValues values, RegexMapping mapping, string input)
    {
        var replacement = valueFormatter.GetFormatted(values, mapping.Key, mapping.Format);
        return Regex.Replace(input, mapping.Value, replacement);
    }
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