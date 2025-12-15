namespace OeSystems.ScriptableHttp.Tests;

public static class TestData
{
    public static Values EmptyValues { get; } = CreateEmptyValues();

    public static Values CreateEmptyValues() => new();
}