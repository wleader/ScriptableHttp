using System.Net.Http;
using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Json;
using OeSystems.ScriptableHttp.Regex;
using OeSystems.ScriptableHttp.Xml;

namespace OeSystems.ScriptableHttp.Response;

public interface IResponseReader
{
    Task<IReadOnlyValues> Read(HttpResponseMessage response, ResponseConfig config);
}

public class ResponseReader(
    IJsonResponseReader jsonResponseReader,
    IRegexResponseReader regexResponseReader,
    IXmlResponseReader xmlResponseReader,
    IResponseHeadersReader headersReader)
    : IResponseReader
{
    public async Task<IReadOnlyValues> Read(HttpResponseMessage response, ResponseConfig config)
    {
        var result = new Values();
        headersReader.Read(response.Headers, config.Header ?? [], result);
        var body = await response.Content.ReadAsStringAsync();
        jsonResponseReader.Read(body, config.Mappings?.JsonPath ?? [], result);
        regexResponseReader.Read(body, config.Mappings?.Regex ?? [], result);
        xmlResponseReader.Read(body, config.Mappings?.XPath ?? [], result);
        return result;
    }
}