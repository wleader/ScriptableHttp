using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Request;

namespace OeSystems.ScriptableHttp.Tests.Request;

[TestClass]
public class ValueFormatterFixture
{
    private ValueFormatter _formatter = null!;
    private readonly Mock<ICultureContext> _cultureContext = new();

    private readonly Values _values = new();
    private const string Key = "Key";

    [TestInitialize]
    public void Initialize()
    {
        _values.Clear();
        _cultureContext.Reset();

        _cultureContext.SetupGet(x => x.Current)
            .Returns(CultureInfo.InvariantCulture);

        _formatter = new(_cultureContext.Object);
    }

    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedParameter.Local
    private class ToStringClass
    {
        public override string ToString() => "NoArgs";
    }

    private class FormatClass
    {
        public string ToString(string format) => "FormatOnly";
    }

    private class ProviderClass
    {
        public string ToString(IFormatProvider? provider) => "ProviderOnly";
    }

    private class FormatAndProviderClass
    {
        public string ToString(IFormatProvider? provider) => "ProviderOnly";
        public string ToString(string format, IFormatProvider? provider) => "FormatAndProvider";
    }

    private void Given_Value(object obj) => _values[Key] = obj;

    private void Verify(string expected, string? format)
    {
        Assert.AreEqual(expected, _formatter.GetFormatted(_values, Key, format));
    }

    [TestMethod]
    public void Given_String_When_GetFormatted_Then_Result()
    {
        const string inputString = "InputString";
        Given_Value(inputString);
        Verify(inputString, null);
        Verify(inputString, "format");
        Verify(inputString, null);
        Verify(inputString, "format");
    }

    [TestMethod]
    public void Given_Int32_When_GetFormatted_Then_Result()
    {
        Given_Value(42);
        Verify("42", null);
        Verify("0042", "0000");
        Given_Value(4242);
        var expected = (4242).ToString("N4");
        Verify(expected, "N4");
        expected = (4242).ToString();
        Verify(expected, null);
    }

    [TestMethod]
    public void Given_UserDefinedType_When_GetFormatted_Then_Result()
    {
        Given_Value(new ToStringClass());
        Verify("NoArgs", null);
        Verify("NoArgs", "format");
        
        Given_Value(new  FormatClass());
        Verify("FormatOnly", "format");
        
        Given_Value(new ProviderClass());
        Verify("ProviderOnly", null);
        Verify("ProviderOnly", "format");
        
        Given_Value(new FormatAndProviderClass());
        Verify("ProviderOnly", null);
        Verify("FormatAndProvider", "format");
    }

    [TestMethod]
    public void Given_KeyNotInValues_When_GetFormatted_Then_Empty()
    {
        _values.Clear();
        Verify("", "format");
    }
}