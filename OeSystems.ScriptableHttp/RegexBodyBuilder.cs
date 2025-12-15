using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IRegexBodyBuilder : IBodyBuilder;

public class RegexBodyBuilder : IRegexBodyBuilder
{
    public string Build(IReadOnlyValues values, PostRequest config)
    {
        throw new System.NotImplementedException();
    }
}