using System.IO;
using System.Xml;

namespace OeSystems.ScriptableHttp.Xml;

public interface IXmlDocumentReader
{
    public XmlDocument Read(string xmlContent);
}

public class XmlDocumentReader : IXmlDocumentReader
{
    private static readonly XmlReaderSettings Settings = new()
    {
        DtdProcessing = DtdProcessing.Ignore, // Ignore any DTD found
        XmlResolver = null, // Prohibit resolving external resources (e.g., external DTD files)
        ValidationType = ValidationType.None // Do not perform validation
    };

    public XmlDocument Read(string xmlContent)
    {
        using var stringReader = new StringReader(xmlContent);
        using var xmlReader = XmlReader.Create(stringReader, Settings);
        var doc = new XmlDocument();
        doc.Load(xmlReader);
        return doc;
    }
}