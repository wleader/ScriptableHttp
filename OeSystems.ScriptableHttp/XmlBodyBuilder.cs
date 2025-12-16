using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IXmlBodyBuilder : IBodyBuilder;

public class XmlBodyBuilder : IXmlBodyBuilder
{
    public string Build(IReadOnlyValues values, PostConfig config)
    {
        throw new System.NotImplementedException();
    }
}