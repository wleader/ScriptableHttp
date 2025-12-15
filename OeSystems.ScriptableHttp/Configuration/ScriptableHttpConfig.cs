using System.IO;
using System.Xml.Serialization;

namespace OeSystems.ScriptableHttp.Configuration;

public partial class ScriptableHttpConfig
{
    private static readonly XmlSerializer Serializer = new(typeof(ScriptableHttpConfig));

    public static ScriptableHttpConfig? Load(Stream stream) =>
        Serializer.Deserialize(stream) as ScriptableHttpConfig;

    public static ScriptableHttpConfig? Load(string xml)
    {
        using var reader = new StringReader(xml);
        return Serializer.Deserialize(reader) as ScriptableHttpConfig;
    }
}