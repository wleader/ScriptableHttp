using System.Net.Http;
using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Json;

namespace OeSystems.ScriptableHttp.Response;

public interface IResponseReader
{
    Task<IReadOnlyValues> Read(HttpResponseMessage response, ResponseConfig config);
}

public class ResponseReader(
    IJsonResponseReader jsonResponseReader)
    : IResponseReader
{
    public Task<IReadOnlyValues> Read(HttpResponseMessage response, ResponseConfig config)
    {
        if (config.Mappings.JsonPathSpecified)
            return jsonResponseReader.Read(response, config);
        
        // if (config.Mappings.RegexSpecified)
        //     return regexBuilder.Build(values, config);
        //
        // if (config.Mappings.JsonPathSpecified)
        //     return jsonBodyBuilder.Build(values,  config);
        //
        // if (config.Mappings.XPathSpecified)
        //     return xmlBodyBuilder.Build(values, config);
        //
        // return config.BodyTemplate.Value;
        
        
        throw new System.NotImplementedException();
    }
}