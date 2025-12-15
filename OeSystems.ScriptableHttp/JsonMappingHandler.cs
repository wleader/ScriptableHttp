using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IJsonMappingHandler
{
    void PerformMap(IReadOnlyValues values, JObject document, JsonPathMapping config);
}

public class JsonMappingHandler(
    IValueFormatter formatter) : IJsonMappingHandler
{
    public void PerformMap(IReadOnlyValues values, JObject document, JsonPathMapping config)
    {
        var formatted = formatter.GetFormatted(
            values,
            config.Key,
            config.Format);

        // todo check if a required match was not found.
        // todo apply regex to path.

        foreach (var token in document.SelectTokens(config.Query))
        {
            // maybe this shouldn't be a string?
            // could it be a number bool or null?
            // should we support arrays and objects?
            token.Replace(formatted);
        }
    }
}