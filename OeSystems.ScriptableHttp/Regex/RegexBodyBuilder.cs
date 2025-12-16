using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Regex;

public interface IRegexBodyBuilder : IBodyBuilder;

public class RegexBodyBuilder(IRegexMapper mapper) : IRegexBodyBuilder
{
    public string Build(IReadOnlyValues values, PostRequest config) => 
        mapper.Replace(values, config.Mappings.Regex, config.BodyTemplate.Value);
}