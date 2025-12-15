using System.Text.RegularExpressions;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IRegexMapper
{
    string Replace(IReadOnlyValues values, RegexMapping mapping, string input);
}

public class RegexMapper(IValueFormatter valueFormatter) :  IRegexMapper
{
    public string Replace(IReadOnlyValues values, RegexMapping mapping, string input)
    {
        var replacement = valueFormatter.GetFormatted(values, mapping.Key, mapping.Format);
        return Regex.Replace(input, mapping.Value, replacement);
    }
}