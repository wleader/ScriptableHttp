using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OeSystems.ScriptableHttp.Tests.Resources;

namespace OeSystems.ScriptableHttp.Tests.Configuration;

[TestClass]
public class ScriptableHttpConfigFixture
{
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