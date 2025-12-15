
namespace OeSystems.ScriptableHttp;

public enum ResultCode
{
    Success = 0,
    Error = 1
}

public readonly record struct Result(ResultCode ResultCode, IReadOnlyValues Values)
{
    public bool IsSuccess => ResultCode == ResultCode.Success;
    public bool IsError => ResultCode != ResultCode.Success;
}
