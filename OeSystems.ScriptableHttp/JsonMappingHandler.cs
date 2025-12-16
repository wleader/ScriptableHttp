using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Request;

namespace OeSystems.ScriptableHttp;

public interface IJsonMappingHandler
{
    void PerformMap(IReadOnlyValues values, JObject document, JsonPathMap config);
}

public class JsonMappingHandler(
    IValueFormatter formatter) : IJsonMappingHandler
{
    public void PerformMap(IReadOnlyValues values, JObject document, JsonPathMap config)
    {
        var formatted = formatter.GetFormatted(
            values,
            config.Key,
            config.Format);

        foreach (var token in document.SelectTokens(config.Query))
        {
            // maybe this shouldn't be a string?
            // could it be a number bool or null?
            // should we support arrays and objects?
            token.Replace(formatted);
        }
    }
}