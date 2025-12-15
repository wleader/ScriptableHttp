using System;
using System.Diagnostics;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Telemetry;

public static class ActivitySources
{
    public static readonly ActivitySource Default = new ActivitySource("OeSystems.ScriptableHttp", "0.1.0");
}

public abstract class BaseActivity(Activity? activity) : IDisposable
{
    public void Dispose()
    {
        activity?.Dispose();
        GC.SuppressFinalize(this);
    }

    public void AddException(Exception exception)
    {
        activity?
            .AddException(exception)
            .AddTag("error.type", exception.GetType().FullName);
    }
}

public class ClientInvokeActivity(ScriptableHttpConfig config)
    : BaseActivity(ActivitySources.Default.StartActivity(
        "invoke " + config.Name,
        ActivityKind.Client));

public class OperationInvokeActivity(OperationConfig config)
    : BaseActivity(ActivitySources.Default.StartActivity(
        "operation " + config.Name,
        ActivityKind.Client));

public class CompileActivity(ScriptableHttpConfig config)
: BaseActivity(ActivitySources.Default.StartActivity(
        "compile " + config.Name,
        ActivityKind.Client));

public class ScriptActivity(ScriptableHttpConfig config)
: BaseActivity(ActivitySources.Default.StartActivity(
        "script " + config.Name,
        ActivityKind.Client));