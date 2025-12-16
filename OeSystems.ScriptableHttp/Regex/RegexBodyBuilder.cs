using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Request;

namespace OeSystems.ScriptableHttp.Regex;

public interface IRegexBodyBuilder : IBodyBuilder;

public class RegexBodyBuilder(IRegexMapper mapper) : IRegexBodyBuilder
{
    public string Build(IReadOnlyValues values, PostConfig config) => 
        mapper.Replace(values, config.Mappings.Regex, config.BodyTemplate.Value);
}