using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class TimeoutTokenFactoryFixture
{
    private TimeoutTokenFactory _factory = null!;
    private readonly Mock<IProvideOperationTimeout> _provideTimeout = new();
    
    private readonly OperationConfig _config = new()
    {
        Name = "Test Operation",
        Request = new()
        {
            Get = new(),
        },
        Response = new(),
    };
    
    [TestInitialize]
    public void Initialize()
    {
        _provideTimeout.Reset();
        _provideTimeout.Setup(x => x.GetTimeout(_config))
            .Returns(TimeSpan.FromMilliseconds(10));
        _factory = new (_provideTimeout.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _factory.Dispose();
    }

    [TestMethod]
    public async Task Given_Token_When_Delay_Then_TokenIsCanceled()
    {
        var t = _factory.GetToken(_config);
        Assert.IsFalse(t.IsCancellationRequested);
        await Task.Delay(20, CancellationToken.None);
        Assert.IsTrue(t.IsCancellationRequested);
    }
}