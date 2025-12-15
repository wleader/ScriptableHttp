using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Tests.Resources;

namespace OeSystems.ScriptableHttp.Tests.Configuration;

[TestClass]
public class ScriptableHttpConfigFixture
{
    private static Assembly? _assembly;
    private static string? _resourcePrefix;
    
    private static Stream? GetResourceStream(string resourceName)
    {
        _assembly ??= Assembly.GetExecutingAssembly();
        _resourcePrefix ??= _assembly.GetName().Name + ".Configuration.";
        var qualifiedName = _resourcePrefix + resourceName;
        return _assembly.GetManifestResourceStream(qualifiedName);
    }

    private static ScriptableHttpConfig GetConfigurationFromResource(string resourceName)
    {
        using var stream = GetResourceStream(resourceName);
        Assert.IsNotNull(stream, $"Could not find resource {resourceName}");
        var result = ScriptableHttpConfig.Load(stream);
        Assert.IsNotNull(result, $"Could not deserialize resource {resourceName}");
        return result;
    }
    
    [TestMethod]
    public void Given_ConfigurationWithXmlRequestTemplate_When_Load_Then_Loaded()
    {
        var config = TestResources.Configuration1();
        var templateString = config.Operation[0].Request.Post.BodyTemplate.Value;
        Console.WriteLine(templateString);
    }
    
    [TestMethod]
    public void Given_ConfigurationWithJsonRequestTemplate_When_Load_Then_Loaded()
    {
        var config = TestResources.Configuration2();
        var templateString = config.Operation[0].Request.Post.BodyTemplate.Value;
        Console.WriteLine(templateString);
    }
}