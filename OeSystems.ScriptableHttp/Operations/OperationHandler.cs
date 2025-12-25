using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OeSystems.ScriptableHttp.Configuration;
using OeSystems.ScriptableHttp.Request;
using OeSystems.ScriptableHttp.Response;
using OeSystems.ScriptableHttp.Telemetry;

namespace OeSystems.ScriptableHttp.Operations;

public interface IOperationHandler
{
    void Reset(IReadOnlyValues inputValues);
    Task<ValuesResult> Invoke(OperationConfig operationConfig);
    ValuesResult LastResult { get; }
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

    public ValuesResult LastResult { get; private set; } = new(Values.Empty, Errors.None);

    public async Task<ValuesResult> Invoke(OperationConfig config)
    {
        await _semaphore.WaitAsync();
        using var activity = new OperationInvokeActivity(config);

        var httpClient = httpClientFactory.CreateClient("ScriptableHttpClient");
        var httpRequest = requestBuilder.Build(values.Combine(), config.Request);
        var token = tokenFactory.GetToken(config);

        HttpResponseMessage? httpResponse;
        ValuesResult result;
        try
        {
            httpResponse = await httpClient.SendAsync(httpRequest, token);
        }
        catch (Exception ex)
        {
            activity.AddException(ex);
            result = LastResult = new(values.Combine(),
                Errors.Create("There was an error while processing the request.", ex));
            _semaphore.Release();
            return result;
        }

        var responseValues = await responseReader.Read(httpResponse, config.Response);
        values.Add(responseValues);
        result = LastResult = new(values.Combine(), Errors.None);
        _semaphore.Release();
        return result;
    }
}