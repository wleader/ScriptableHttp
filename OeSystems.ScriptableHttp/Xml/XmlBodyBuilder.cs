using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Request;

namespace OeSystems.ScriptableHttp.Xml;

public interface IXmlBodyBuilder : IBodyBuilder;

public class XmlBodyBuilder : IXmlBodyBuilder
{
    public string Build(IReadOnlyValues values, PostConfig config)
    {
        throw new System.NotImplementedException();
    }
}