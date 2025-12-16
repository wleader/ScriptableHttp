using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp;

public interface IStringContentBuilder
{
    StringContent Build(IReadOnlyValues values, PostConfig request);
}

/// <summary>
/// Converts a body string into a string content
/// using an encoding appropriate to the content type.
/// </summary>
public class StringContentBuilder(
    IBodyBuilder bodyBuilder)
    : IStringContentBuilder
{
    public StringContent Build(IReadOnlyValues values, PostConfig request)
    {
        var ct = MediaTypeHeaderValue.Parse(request.BodyTemplate.ContentType);
        var encoding = string.IsNullOrEmpty(ct.CharSet)
            ? Encoding.UTF8 
            : Encoding.GetEncoding(ct.CharSet);
        var content = bodyBuilder.Build(values, request);
        return new (content, encoding, ct.MediaType);
    }
}