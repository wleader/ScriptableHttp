using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Regex;

public interface IRegexMapperSingle
{
    string Replace(IReadOnlyValues values, RegexMapping mapping, string input);
}

public class RegexMapperSingle(IValueFormatter valueFormatter) : IRegexMapperSingle
{
    public string Replace(IReadOnlyValues values, RegexMapping mapping, string input)
    {
        var replacement = valueFormatter.GetFormatted(values, mapping.Key, mapping.Format);
        return System.Text.RegularExpressions.Regex.Replace(input, mapping.Value, replacement);
    }
}