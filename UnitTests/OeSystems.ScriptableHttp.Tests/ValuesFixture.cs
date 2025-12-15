using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class ValuesFixture
{
    [TestMethod]
    public void Given_NullData_When_New_Then_ValuesAreEmpty()
    {
        Assert.AreEqual(0, new Values().Count);
    }

    [TestMethod]
    public void Given_New_When_SetValue_Then_KeysAreCaseInsensitive()
    {
        var actual = new Values
        {
            ["Key1"] = "Value1"
        };
        Assert.AreEqual("Value1", actual["key1"]);
    }

    [TestMethod]
    public void Given_ExistingData_When_Add_Then_ValuesAreMerged()
    {
        var existing = new Values
        {
            ["Key1"] = "OriginalValue1",
            ["Key2"] = "OriginalValue2",
        };

        var toAdd = new Values
        {
            ["key2"] = "NewValue2",
            ["key3"] = "NewValue3",
        };
        
        existing.Add(toAdd);
        
        Assert.AreEqual(3, existing.Count);
        Assert.AreEqual("OriginalValue1", existing["key1"]); // key1 unchanged
        Assert.AreEqual("NewValue2", existing["key2"]); // key2 replaced
        Assert.AreEqual("NewValue3", existing["key3"]); // key3 added
    }
}