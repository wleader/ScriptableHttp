using System;
using System.Threading;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Operations;

public interface ITimeoutTokenFactory : IDisposable
{
    CancellationToken GetToken(OperationConfig config);
}

public class TimeoutTokenFactory(
    IProvideOperationTimeout provideTimeout)
    : ITimeoutTokenFactory
{
    private CancellationTokenSource? _cts;

    public CancellationToken GetToken(OperationConfig config)
    {
        var configuredTimeout = provideTimeout.GetTimeout(config);
        _cts?.Dispose();
        _cts = new (configuredTimeout);
        return _cts.Token;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _cts?.Dispose();
    }
}
