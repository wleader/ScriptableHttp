using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OeSystems.ScriptableHttp.Configuration;

namespace OeSystems.ScriptableHttp.Tests;

[TestClass]
public class RequestBuilderFixture
{
    private RequestBuilder _builder = null!;
    private readonly Mock<IPostRequestBuilder> _postBuilder = new();
    private readonly Mock<IGetRequestBuilder> _getBuilder = new();
    private static readonly HttpRequestMessage PostBuilderResult = new();
    private static readonly HttpRequestMessage GetBuilderResult = new();
    private static readonly Values Values = TestData.CreateEmptyValues();
    private static readonly GetConfig GetConfig = new();
    private static readonly PostConfig PostConfig = new();

    [TestInitialize]
    public void Initialize()
    {
        _postBuilder.Reset();
        _getBuilder.Reset();

        _postBuilder.Setup(x => x.Build(Values, PostConfig))
            .Returns(() => PostBuilderResult);
        _getBuilder.Setup(x => x.Build(Values, GetConfig))
            .Returns(() => GetBuilderResult);

        _builder = new(_postBuilder.Object, _getBuilder.Object);
    }

    [TestMethod]
    public void Given_PostConfiguration_When_Build_Then_ResultIsPostResult()
    {
        var actual = _builder.Build(Values, new() { Post = PostConfig });
        Assert.AreSame(PostBuilderResult, actual);
    }

    [TestMethod]
    public void Given_GetConfiguration_When_Build_Then_ResultIsPostResult()
    {
        var actual = _builder.Build(Values, new() { Get = GetConfig });
        Assert.AreSame(GetBuilderResult, actual);
    }

    [TestMethod]
    public void Given_NullConfiguration_When_Build_Then_Throws()
    {
        Assert.ThrowsException<ApplicationException>(() => _builder.Build(Values, new()));
    }
}