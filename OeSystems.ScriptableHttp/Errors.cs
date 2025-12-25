using System;
using System.Collections.Generic;

namespace OeSystems.ScriptableHttp;

public readonly record struct Error(string Message, Exception? Exception = null);

public interface IReadOnlyErrors : IReadOnlyList<Error>;

public interface IErrors : IReadOnlyErrors;

public class Errors : List<Error>, IErrors
{
    public static IReadOnlyErrors None { get; } = new Errors();
    public static IReadOnlyErrors Create(string message, Exception? exception = null) =>
        new Errors{new(message, exception)};
}
