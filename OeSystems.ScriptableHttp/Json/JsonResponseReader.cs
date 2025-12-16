using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Response;

namespace OeSystems.ScriptableHttp.Json;

public interface IJsonResponseReader : IResponseReader;

public class JsonResponseReader : IJsonResponseReader
{
    public async Task<IReadOnlyValues> Read(HttpResponseMessage response, ResponseConfig config)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        var result = new Values();
        
        var document = JObject.Parse(responseBody);
        foreach (var jsonPathConfig in config.Mappings.JsonPath)
        {
            var token = document.SelectToken(jsonPathConfig.Query);
            if (token == null && jsonPathConfig.Required)
            {
                // problem
                throw new NotImplementedException();
            }
            
            if (token == null)
                continue;

            var type = Type.GetType(jsonPathConfig.DataType) ?? typeof(string);
            result[jsonPathConfig.Key] = token.ToObject(type);
        }

        return result;
    }
}
