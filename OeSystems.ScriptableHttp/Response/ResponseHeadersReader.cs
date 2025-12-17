using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Response;

public interface IResponseHeadersReader
{
    void Read(HttpResponseHeaders header, IEnumerable<ResponseHeaderConfig> config, Values values);
}

public class ResponseHeadersReader : IResponseHeadersReader
{
    public void Read(HttpResponseHeaders headers, IEnumerable<ResponseHeaderConfig> config, Values values)
    {
        throw new System.NotImplementedException();
    }
}