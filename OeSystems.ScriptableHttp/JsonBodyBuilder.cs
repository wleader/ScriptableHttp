using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IJsonBodyBuilder : IBodyBuilder;

public class JsonBodyBuilder(
    IJsonMappingHandler handler) : IJsonBodyBuilder
{
    public string Build(IReadOnlyValues values, PostRequest config)
    {
        var document = JObject.Parse(config.BodyTemplate.Value);
        foreach (var jsonPathConfig in config.Mappings.JsonPath)
        {
            handler.PerformMap(values, document, jsonPathConfig);
        }
        return document.ToString();
    }
}