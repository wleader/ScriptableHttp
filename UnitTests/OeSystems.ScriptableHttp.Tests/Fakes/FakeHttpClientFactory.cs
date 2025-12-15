using System;
using System.Net.Http;
using Moq;

namespace OeSystems.ScriptableHttp.Tests.Fakes;

public class FakeHttpClientFactory : IHttpClientFactory
{
    private readonly Mock<IHttpClientFactory> _mock = new();

    public FakeHttpClientHandler Handler { get; } = new();

    public FakeHttpClientFactory()
    {
        Reset();
    }

    public HttpClient CreateClient(string name) =>
        _mock.Object.CreateClient(name);

    public void Reset()
    {
        _mock.Reset();
        Handler.Reset();
        _mock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(() => new(Handler));
    }

    public void SetupSimple(HttpRequestMessage request, Func<HttpResponseMessage> func) => 
        Handler.SetupSimple(request, func);
    
    public void SetupSendThrows(Exception ex) =>
        Handler.SetupSendThrows(ex);
    
    public void VerifySentOnly(HttpRequestMessage request) => 
        Handler.VerifySentOnly(request);

    public void VerifyCreatedOnce(string clientName = "") => 
        _mock.Verify(x => x.CreateClient(clientName), Times.Once);
}