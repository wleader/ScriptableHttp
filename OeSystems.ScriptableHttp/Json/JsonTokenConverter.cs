using System;
using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Response;

namespace OeSystems.ScriptableHttp.Json;

public interface IJsonTokenConverter
{
    ObjectResult Convert(JToken token, JsonPathMap map);
}

public class JsonTokenConverter(IValueParser parser) : IJsonTokenConverter
{
    public ObjectResult Convert(JToken token, JsonPathMap map)
    {
        var type = Type.GetType(map.DataType); // this could be cached
        
        if (type == null)
        {
            return new(null, Errors.Create($"'{map.DataType}' is not a recognized data type."));
        }

        try
        {
            return new(token.ToObject(type), Errors.None);
        }
        catch (Exception ex)
        {
            var stringValue = ((token as JValue)?.Value as string);
            return stringValue == null
                ? ObjectResult.Fail("'JToken did not have a value.", ex)
                : parser.Parse(stringValue, map);
        }
    }
}