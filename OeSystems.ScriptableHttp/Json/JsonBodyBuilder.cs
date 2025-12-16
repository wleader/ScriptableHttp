using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Request;

namespace OeSystems.ScriptableHttp.Json;

public interface IJsonBodyBuilder : IBodyBuilder;

public class JsonBodyBuilder(
    IJsonMappingHandler handler) : IJsonBodyBuilder
{
    public string Build(IReadOnlyValues values, PostConfig config)
    {
        var document = JObject.Parse(config.BodyTemplate.Value);
        foreach (var jsonPathConfig in config.Mappings.JsonPath)
        {
            handler.PerformMap(values, document, jsonPathConfig);
        }
        return document.ToString();
    }
}