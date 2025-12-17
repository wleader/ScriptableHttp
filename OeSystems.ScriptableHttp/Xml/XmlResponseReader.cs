using System.Collections.Generic;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Xml;

public interface IXmlResponseReader
{
    void Read(string body, IEnumerable<XPathMap> mappings, Values values);
}

public class XmlResponseReader : IXmlResponseReader
{
    public void Read(string body, IEnumerable<XPathMap> mappings, Values values)
    {
        throw new System.NotImplementedException();
    }
}