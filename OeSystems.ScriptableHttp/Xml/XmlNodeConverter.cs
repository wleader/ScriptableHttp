using System.Xml;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Response;

namespace OeSystems.ScriptableHttp.Xml;

public interface IXmlNodeConverter
{
    ObjectResult Convert(XmlNode? node, XPathMap map);
}

public class XmlNodeConverter(IValueParser parser) : IXmlNodeConverter
{
    public ObjectResult Convert(XmlNode? node, XPathMap map)
    {
        return parser.Parse(node?.Value, map);
    }
}