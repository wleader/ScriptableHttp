using Microsoft.VisualStudio.TestTools.UnitTesting;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class ProvideOperationTimeoutFixture
{
    private readonly ProviderOperationTimeout _provider = new();

    private readonly OperationConfig _config = new()
    {
        Name = "Test Operation",
        Request = new(),
        Response = new(),
    };
    
    [TestMethod]
    public void Given_GetRequest_When_GetTimeout_Then_Result()
    {
        _config.Request.Get = new() { TimeoutSeconds = 60 };
        var actual = _provider.GetTimeout(_config);
        Assert.AreEqual(60, actual.TotalSeconds);
    }
    
    [TestMethod]
    public void Given_PostRequest_When_GetTimeout_Then_Result()
    {
        _config.Request.Post = new() { TimeoutSeconds = 61 };
        var actual = _provider.GetTimeout(_config);
        Assert.AreEqual(61, actual.TotalSeconds);
    }
}