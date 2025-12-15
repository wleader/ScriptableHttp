using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests.Resources;

public static class TestResources
{
    private static Assembly? _assembly;
    private static string? _resourcePrefix;

    private static Stream GetResourceStream(string resourceName)
    {
        _assembly ??= Assembly.GetExecutingAssembly();
        _resourcePrefix ??= _assembly.GetName().Name + ".Resources.";
        var qualifiedName = _resourcePrefix + resourceName;
        var result = _assembly.GetManifestResourceStream(qualifiedName);
        Assert.IsNotNull(result, "Could not find resource " + resourceName);
        return result;
    }

    private static ScriptableHttpConfig GetConfig(string name)
    {
        var stream = GetResourceStream("Configurations." + name);
        var config = ScriptableHttpConfig.Load(stream);
        Assert.IsNotNull(config, "Could not load config " + name);
        return config;
    }

    private static Stream GetResponseStream(string name) =>
        GetResourceStream("Responses." + name);
    
    public static ScriptableHttpConfig Configuration1() => GetConfig("Configuration1.xml");
    public static ScriptableHttpConfig Configuration2() => GetConfig("Configuration2.xml");
    public static ScriptableHttpConfig LibraryIntegration1() => GetConfig("LibraryIntegration1.xml");
    public static Stream BasicGetResponseJson() => GetResponseStream("BasicGetResponse.json");
}