using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace OeSystems.ScriptableHttp.Tests.Fakes;

public interface ISendAsync
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken);
}

public class FakeHttpClientHandler : HttpClientHandler
{
    public Mock<ISendAsync> Mock { get; }= new();

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken) => 
            Mock.Object.SendAsync(request, cancellationToken);

    public void SetupSimple(HttpRequestMessage request, Func<HttpResponseMessage> func) =>
        Mock.Setup(x => x.SendAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(func);

    public void VerifySentOnly(HttpRequestMessage request)
    {
        Mock.Verify(x => x.SendAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        Mock.VerifyNoOtherCalls();
    }

    public void SetupSendThrows(Exception ex) =>
        Mock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);
    
    public void Reset() => Mock.Reset();
}