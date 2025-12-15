using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class RegistrationFixture
{
    [TestMethod]
    public void When_Register_Then_ContainerIsValid()
    {
        var serviceCollection = new ServiceCollection();
        
        // the application should add HttpClient
        serviceCollection.AddHttpClient();
        
        serviceCollection.AddScriptableHttp();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        _ = serviceProvider.GetRequiredService<IScriptableHttpClient>();
    }
}