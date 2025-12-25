
namespace OeSystems.ScriptableHttp;

public readonly record struct Result(IReadOnlyErrors Errors)
{
    public bool IsSuccess => Errors.Count == 0;
    public bool IsError => Errors.Count > 0;
    public static readonly Result Success = new (ScriptableHttp.Errors.None);
}

public readonly record struct Result<T>(T Value, IReadOnlyErrors Errors)
{
    public bool IsSuccess => Errors.Count == 0;
    public bool IsError => Errors.Count > 0;
    public static Result<T> Success(T value) => new (value, ScriptableHttp.Errors.None);
}