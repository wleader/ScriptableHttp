using Moq;

namespace OeSystems.ScriptableHttp.Tests.Fakes;

public class FakeFormatter : IValueFormatter
{
    public Mock<IValueFormatter> Mock { get; } = new();

    public void Reset()
    {
        Mock.Reset();
        Mock.Setup(x => x.GetFormatted(
                It.IsAny<IReadOnlyValues>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns((IReadOnlyValues v, string k, string? _) =>
                v.TryGetValue(k, out var r)
                    ? r?.ToString() ?? string.Empty
                    : string.Empty);
    }

    public string GetFormatted(IReadOnlyValues values, string key, string? format) => 
        Mock.Object.GetFormatted(values, key, format);
}