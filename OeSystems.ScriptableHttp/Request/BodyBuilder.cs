using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Json;
using OeSystems.ScriptableHttp.Regex;
using OeSystems.ScriptableHttp.Xml;

namespace OeSystems.ScriptableHttp.Request;

public interface IBodyBuilder
{
    string Build(IReadOnlyValues values, PostConfig config);
}

public class BodyBuilder(
    IRegexBodyBuilder regexBuilder,
    IJsonBodyBuilder jsonBodyBuilder,
    IXmlBodyBuilder xmlBodyBuilder) : IBodyBuilder
{
    public string Build(IReadOnlyValues values, PostConfig config)
    {
        if (config.Mappings.RegexSpecified)
            return regexBuilder.Build(values, config);
        
        if (config.Mappings.JsonPathSpecified)
            return jsonBodyBuilder.Build(values,  config);

        if (config.Mappings.XPathSpecified)
            return xmlBodyBuilder.Build(values, config);

        return config.BodyTemplate.Value;
    }
}