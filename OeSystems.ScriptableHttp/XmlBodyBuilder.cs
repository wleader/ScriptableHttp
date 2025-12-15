using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IXmlBodyBuilder : IBodyBuilder;

public class XmlBodyBuilder : IXmlBodyBuilder
{
    public string Build(IReadOnlyValues values, PostRequest config)
    {
        throw new System.NotImplementedException();
    }
}