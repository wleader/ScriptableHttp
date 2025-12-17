using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Response;

namespace OeSystems.ScriptableHttp.Json;

public interface IJsonResponseReader
{
    public void Read(string body, IEnumerable<JsonPathMap> mappings, Values values);
}

public class JsonResponseReader : IJsonResponseReader
{
    public void Read(string body, IEnumerable<JsonPathMap> mappings, Values values)
    {
        var document = JObject.Parse(body);
        foreach (var mapping in mappings)
        {
            var token = document.SelectToken(mapping.Query);
            if (token == null && mapping.Required)
            {
                // problem
                throw new NotImplementedException();
            }
            
            if (token == null)
                continue;

            var type = Type.GetType(mapping.DataType) ?? typeof(string);
            values[mapping.Key] = token.ToObject(type);
        }
    }
}
