using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Request;
using OeSystems.ScriptableHttp.Telemetry;

namespace OeSystems.ScriptableHttp.Operations;

public interface IOperationHandler
{
    void Reset(IReadOnlyValues inputValues);
    Task<Result> Invoke(OperationConfig operationConfig);
    Result LastResult { get; }
}

public class OperationHandler(
    IValueSet values,
    IRequestBuilder requestBuilder,
    IHttpClientFactory httpClientFactory,
    IResponseReader responseReader,
    ITimeoutTokenFactory tokenFactory)
    : IOperationHandler
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void Reset(IReadOnlyValues inputValues) => values.Reset(inputValues);

    public Result LastResult { get; private set; } = new(ResultCode.Error, new Values());

    public async Task<Result> Invoke(OperationConfig config)
    {
        await _semaphore.WaitAsync();
        using var activity = new OperationInvokeActivity(config);

        var httpClient = httpClientFactory.CreateClient("ScriptableHttpClient");
        var httpRequest = requestBuilder.Build(values.Combine(), config.Request);
        var token = tokenFactory.GetToken(config);

        HttpResponseMessage? httpResponse;
        Result result;
        try
        {
            httpResponse = await httpClient.SendAsync(httpRequest, token);
        }
        catch (Exception ex)
        {
            activity.AddException(ex);
            // log something? 
            // put the exception in the result object?
            result = LastResult = new(ResultCode.Error, values.Combine());
            _semaphore.Release();
            return result;
        }

        var responseValues = await responseReader.Read(httpResponse, config.Response);
        values.Add(responseValues);
        result = LastResult = new(ResultCode.Success, values.Combine());
        _semaphore.Release();
        return result;
    }
}