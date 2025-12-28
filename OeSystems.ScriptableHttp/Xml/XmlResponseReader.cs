using System.Collections.Generic;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Xml;

public interface IXmlResponseReader
{
    ValuesResult Read(string body, IEnumerable<XPathMap> mappings, Values values);
}

public class XmlResponseReader(IXmlReadSingle readSingle, IXmlDocumentReader documentReader) : IXmlResponseReader
{
    public ValuesResult Read(string body, IEnumerable<XPathMap> mappings, Values values)
    {
        Errors errors = [];
        var doc = documentReader.Read(body);
        foreach (var mapping in mappings)
        {
            errors.AddRange(readSingle.Read(doc, mapping, values).Errors);
        }
        return new(values,  errors);
    }
}