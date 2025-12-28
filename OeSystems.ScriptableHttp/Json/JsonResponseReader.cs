using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Json;

public interface IJsonResponseReader
{
    public Result Read(string body, IEnumerable<JsonPathMap> mappings, Values values);
}

public class JsonResponseReader(IJsonResponseReadSingle single) : IJsonResponseReader
{
    public Result Read(string body, IEnumerable<JsonPathMap> mappings, Values values)
    {
        Errors errors = [];
        var document = JObject.Parse(body);
        foreach (var mapping in mappings)
        {
            single.Read(document, mapping, values, errors);
        }
        return errors.Any() ? new(errors) : Result.Success;
    }
}