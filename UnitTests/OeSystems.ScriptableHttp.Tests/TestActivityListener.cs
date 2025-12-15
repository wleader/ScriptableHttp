using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OeSystems.ScriptableHttp.Tests;

public interface ITestActivityListener : IDisposable
{
    IReadOnlyList<Activity> Started { get; }
    IReadOnlyList<Activity> Stopped { get; }
    Activity ExpectOneCompleteActivity();
    void AssertNoActivity();
}

public class TestActivityListener : ITestActivityListener
{
    private readonly ActivityListener _listener;

    private readonly List<Activity> _started = [];
    public IReadOnlyList<Activity> Started => _started;

    private readonly List<Activity> _stopped = [];
    public IReadOnlyList<Activity> Stopped => _stopped;

    private static ActivitySamplingResult SampleAllData(
        ref ActivityCreationOptions<ActivityContext> options) =>
        ActivitySamplingResult.AllData;

    public TestActivityListener(ActivitySource source)
    {
        _listener = new()
        {
            ShouldListenTo = s => ReferenceEquals(s, source),
            Sample = SampleAllData,
            ActivityStarted = _started.Add,
            ActivityStopped = (a) =>
            {
                _started.Remove(a);
                _stopped.Add(a);
            },
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public Activity ExpectOneCompleteActivity()
    {
        Assert.AreEqual(0, Started.Count, "There are incomplete activities.");
        Assert.AreEqual(1, Stopped.Count, "There is not exactly 1 completed activity.");
        return Stopped[0];
    }

    public void AssertNoActivity()
    {
        Assert.AreEqual(0, Started.Count, "There are incomplete activities.");
        Assert.AreEqual(0, Stopped.Count, "There are complete activities.");
    }

    public void Dispose()
    {
        _listener.Dispose();
        GC.SuppressFinalize(this);
    }
}