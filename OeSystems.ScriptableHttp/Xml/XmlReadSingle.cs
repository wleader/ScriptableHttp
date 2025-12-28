using System;
using System.Xml;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Regex;

namespace OeSystems.ScriptableHttp.Xml;

public interface IXmlReadSingle
{
    Result Read(XmlDocument doc, XPathMap mapping, Values values);
}

public class XmlReadSingle(
    IRegexMapper regexMapper,
    IXmlNodeConverter converter) : IXmlReadSingle
{
    public Result Read(XmlDocument doc, XPathMap mapping, Values values)
    {
        var query = regexMapper.Replace(values, mapping.Regex, mapping.Query);
        try
        {
            var nodes = doc.SelectNodes(query);
            if (nodes is null || nodes.Count == 0)
            {
                return mapping.Required 
                    ? Result.Fail($"The Query '{query}' did not have any matches.") 
                    : Result.Success;
            }

            if (nodes.Count > 1)
                return Result.Fail($"The Query '{query}' has more than one match.");
            
            var result = converter.Convert(nodes[0], mapping);
            if (result.IsSuccess)
            {
                values[mapping.Key] = result.Value;
            }
            return new(result.Errors);
        }
        catch (Exception ex)
        {
            return Result.Fail("There was an unhandled exception.", ex);
        }
    }
}