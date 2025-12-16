using System;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Operations;

public interface IProvideOperationTimeout
{
    TimeSpan GetTimeout(OperationConfig config);
}

public class ProviderOperationTimeout : IProvideOperationTimeout
{
    public TimeSpan GetTimeout(OperationConfig config)
    {
        return TimeSpan.FromSeconds(
            (config.Request.Get ?? config.Request.Post).TimeoutSeconds);
    }
}