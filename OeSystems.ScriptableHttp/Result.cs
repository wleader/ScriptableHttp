
using System;

namespace OeSystems.ScriptableHttp;

public readonly record struct Result(IReadOnlyErrors Errors)
{
    public bool IsSuccess => Errors.Count == 0;
    public bool IsError => Errors.Count > 0;
    public static readonly Result Success = new (ScriptableHttp.Errors.None);
    public static Result Fail(string message, Exception? exception = null) =>
        new(new Errors{new(message, exception)});
}

public readonly record struct Result<T>(T? Value, IReadOnlyErrors Errors)
{
    public bool IsSuccess => Errors.Count == 0;
    public bool IsError => Errors.Count > 0;
    public static Result<T> Success(T value) => new (value, ScriptableHttp.Errors.None);
    public static Result<T> Fail(string message, Exception? exception = null) =>
        new(default, new Errors{new(message, exception)});
}