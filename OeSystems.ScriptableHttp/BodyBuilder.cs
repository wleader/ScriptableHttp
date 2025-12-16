using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Regex;

namespace OeSystems.ScriptableHttp;

public interface IBodyBuilder
{
    string Build(IReadOnlyValues values, PostRequest config);
}

public class BodyBuilder(
    IRegexBodyBuilder regexBuilder,
    IJsonBodyBuilder jsonBodyBuilder,
    IXmlBodyBuilder xmlBodyBuilder) : IBodyBuilder
{
    public string Build(IReadOnlyValues values, PostRequest config)
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