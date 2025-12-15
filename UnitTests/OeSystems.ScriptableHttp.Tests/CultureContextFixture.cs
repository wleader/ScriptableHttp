using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class CultureContextFixture
{
    private readonly CultureContext _context = new();
    
    [TestMethod]
    public void Given_enUS_When_Set_Then_Current()
    {
        var initial = _context.Current;
        
        var actual = _context.Set("en-US");
        Assert.IsNotNull(actual);
        Assert.AreNotEqual(initial, _context.Current);
        Assert.AreEqual("en-US", _context.Current.Name);
        actual.Dispose();
        Assert.AreEqual(initial, _context.Current);
    }

    [TestMethod]
    [DataRow("NotAValidCulture", DisplayName = "InvalidCulture")]
    [DataRow("", DisplayName = "Empty")]
    [DataRow("", DisplayName = "Whitespace")]
    [DataRow((string)null!, DisplayName = "Null")]
    public void Given_NotAValidCulture_When_GetCulture_Then_InvariantCulture(string cultureName)
    {
        using var _ = _context.Set(cultureName);
        Assert.AreEqual(CultureInfo.InvariantCulture, _context.Current);
    }

    [TestMethod]
    public void When_MultipleSet_And_Dispose_Then_CurrentIsStacked()
    {
        Assert.AreEqual(CultureInfo.InvariantCulture, _context.Current);
        var disposable1 = _context.Set("en-US");
        Assert.AreNotEqual(CultureInfo.InvariantCulture, _context.Current);
        var disposable2 = _context.Set("en-US");
        Assert.AreNotEqual(CultureInfo.InvariantCulture, _context.Current);
        disposable2.Dispose();
        Assert.AreNotEqual(CultureInfo.InvariantCulture, _context.Current);
        disposable1.Dispose();
        Assert.AreEqual(CultureInfo.InvariantCulture, _context.Current);
    }
}