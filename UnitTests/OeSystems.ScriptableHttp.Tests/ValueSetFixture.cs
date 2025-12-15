using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class ValueSetFixture
{
    [TestMethod]
    public void Given_New_When_Combine_Then_Empty()
    {
        var valueSet = new ValueSet();
        var actual = valueSet.Combine();
        Assert.AreEqual(0, actual.Count);
    }

    [TestMethod]
    public void Given_New_When_Reset_And_Combine_Then_ResultIsCaseInsensitive()
    {
        var valueSet = new ValueSet();

        var initialValues = new Values
        {
            ["Key1"] = "Value1",
            ["Key2"] = "Value2",
        };
        
        valueSet.Reset(initialValues);
        
        var actual = valueSet.Combine();
        Assert.AreEqual(2, actual.Count);
        Assert.AreEqual("Value1", actual["key1"]);
        Assert.AreEqual("Value2", actual["key2"]);
    }

    [TestMethod]
    public void Given_Initial_When_Add_And_Add_And_Combine_Then_ResultIsLayered()
    {
        var initialValues = new Values
        {
            ["NotReplaced"] = "Initial",
            ["ReplaceIn1"] = "Initial",
            ["ReplaceIn2"] = "Initial",
            ["ReplaceIn1And2"] = "Initial",
        };

        var toAdd1 = new Values
        {
            ["ReplaceIn1"] = "ReplacedInOne",
            ["ReplaceIn1And2"] = "ReplacedInOne",
        };

        var toAdd2 = new Values
        {
            ["ReplaceIn2"] = "ReplacedInTwo",
            ["ReplaceIn1And2"] = "ReplacedInTwo",
        };
        
        var valueSet = new ValueSet();
        valueSet.Reset(initialValues);
        valueSet.Add(toAdd1);
        valueSet.Add(toAdd2);
        
        var actual = valueSet.Combine();
        Assert.AreEqual(4, actual.Count);
        Assert.AreEqual("Initial", actual["NotReplaced"]);
        Assert.AreEqual("ReplacedInOne", actual["ReplaceIn1"]);
        Assert.AreEqual("ReplacedInTwo", actual["ReplaceIn2"]);
        Assert.AreEqual("ReplacedInTwo", actual["ReplaceIn1And2"]);
    }
}