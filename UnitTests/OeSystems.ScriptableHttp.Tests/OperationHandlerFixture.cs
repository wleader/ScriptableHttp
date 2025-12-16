using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Request;
using OeSystems.ScriptableHttp.Tests.Fakes;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class OperationHandlerFixture
{
    private OperationHandler _handler = null!;
    private readonly Mock<IValueSet> _valueSet = new();
    private readonly Mock<IRequestBuilder> _requestBuilder = new();
    private readonly FakeHttpClientFactory _httpClientFactory = new();
    private readonly Mock<IResponseReader> _responseReader = new();
    private readonly Mock<ITimeoutTokenFactory> _tokenFactory = new();

    private readonly Queue<IReadOnlyValues> _combinedValuesQueue = new();
    private readonly Values _combineResult1 = TestData.CreateEmptyValues();
    private readonly Values _combineResult2 = TestData.CreateEmptyValues();
    private readonly HttpRequestMessage _builderResult = new()
    {
        RequestUri = new("https://localhost"),
    };

    private readonly OperationConfig _operationConfig = new()
    {
        Name = "Test Operation",
        Request = new()
        {
            Get = new(),
        },
        Response = new(),
    };

    private readonly HttpResponseMessage _httpResponse = new()
    {
        StatusCode = HttpStatusCode.OK,
    };
    
    private readonly Values _readerResultValues = TestData.CreateEmptyValues();

    private CancellationTokenSource _cts = null!;
    
    [TestInitialize]
    public void Initialize()
    {
        _valueSet.Reset();
        _requestBuilder.Reset();
        _httpClientFactory.Reset();
        _responseReader.Reset();
        
        _combinedValuesQueue.Clear();
        _combinedValuesQueue.Enqueue(_combineResult1);
        _combinedValuesQueue.Enqueue(_combineResult2);

        _valueSet.Setup(x => x.Combine())
            .Returns(() => _combinedValuesQueue.TryDequeue(out var v) 
                ? v
                : throw new ApplicationException("Too many calls to IValueSet.Combine()."));
        _requestBuilder.Setup(x => x.Build(_combineResult1, _operationConfig.Request))
            .Returns(() => _builderResult);
        _httpClientFactory.SetupSimple(_builderResult, () => _httpResponse);
        _responseReader.Setup(x => x.Read(_httpResponse, _operationConfig.Response))
            .ReturnsAsync(() => _readerResultValues);

        _cts = new();
        _tokenFactory.Setup(x => x.GetToken(_operationConfig))
            .Returns(() => _cts.Token);
        
        _handler = new(
            _valueSet.Object,
            _requestBuilder.Object,
            _httpClientFactory,
            _responseReader.Object,
            _tokenFactory.Object);
    }

    [TestMethod]
    public void When_Reset_Then_ValuesAreReset()
    {
        _handler.Reset(TestData.EmptyValues);
        _valueSet.Verify(x => x.Reset(TestData.EmptyValues), Times.Once);
        _valueSet.Verify(x => x.Add(It.IsAny<IReadOnlyValues>()), Times.Never);
    }

    [TestMethod]
    public async Task When_Invoke_Then_ResultIsSuccess_And_ResultValuesAreCombinedValues()
    {
        var actual = await _handler.Invoke(_operationConfig);
        Assert.IsTrue(actual.IsSuccess);
        Assert.AreSame(_combineResult2, actual.Values);
        
        _requestBuilder.Verify(x => x.Build(_combineResult1, _operationConfig.Request), Times.Once);
        _requestBuilder.VerifyNoOtherCalls();
        _httpClientFactory.VerifySentOnly(_builderResult);
        _responseReader.Verify(x => x.Read(_httpResponse, _operationConfig.Response), Times.Once);
        _responseReader.VerifyNoOtherCalls();
        _valueSet.Verify(x => x.Add(_readerResultValues), Times.Once);
        _valueSet.Verify(x => x.Combine(), Times.Exactly(2));
    }

    [TestMethod]
    public async Task When_Invoke_Then_HttpClientIsCreatedOnce()
    {
        _ = await _handler.Invoke(_operationConfig);
        _httpClientFactory.VerifyCreatedOnce("ScriptableHttpClient");
    }

    [TestMethod]
    public async Task Given_HttpClientWillThrow_When_Invoke_Then_ResultIsError()
    {
        var ex = new Exception();
        _httpClientFactory.SetupSendThrows(ex);
        var actual = await _handler.Invoke(_operationConfig);
        Assert.IsTrue(actual.IsError);
    }
    
    [TestMethod]
    public async Task Given_HttpClientWillThrow_When_Invoke_Then_TraceHasException()
    {
        using var listener = new TestActivityListener(Telemetry.ActivitySources.Default);
        var ex = new Exception();
        _httpClientFactory.SetupSendThrows(ex);
        _ = await _handler.Invoke(_operationConfig);

        var activity = listener.ExpectOneCompleteActivity();
        var tag =  activity.Tags.Single(t => t.Key == "error.type");
        Assert.AreEqual("System.Exception", tag.Value);
    }
    
    [TestMethod]
    public async Task When_Invoke_Then_TraceActivity()
    {
        using var listener = new TestActivityListener(Telemetry.ActivitySources.Default);
        _ = await _handler.Invoke(_operationConfig);
        var actual = listener.ExpectOneCompleteActivity();
        Assert.AreEqual("operation " + _operationConfig.Name, actual.OperationName);
    }

    [TestMethod]
    public async Task When_Invoke_Then_RequestWasSent()
    {
        _ = await _handler.Invoke(_operationConfig);
        _httpClientFactory.VerifySentOnly(_builderResult);
    }

    [TestMethod]
    public async Task When_Invoke_Then_TimeoutTokenWasUsed()
    {
        // Because HttpClient creates its own
        // Linked Token from the one we pass in, we can't verify the
        // token that made it to the FakeHttpClientHandler.
        // but if we make our token factory return a canceled token,
        // then the HTTPClient will throw a TaskCanceledException, 
        // so checking for that exception proves that the token from
        // the token factory was used.
        await _cts.CancelAsync();
        var actual = await _handler.Invoke(_operationConfig);
        Assert.IsFalse(actual.IsSuccess);
    }
}