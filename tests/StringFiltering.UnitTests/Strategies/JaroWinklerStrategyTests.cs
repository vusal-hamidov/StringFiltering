using StringFiltering.Domain.Strategies;

namespace StringFiltering.UnitTests.Strategies;

public class JaroWinklerStrategyTests
{
    private readonly JaroWinklerStrategy _strategy = new();

    [Fact]
    public void GetSimilarityExactMatchReturnsOne()
    {
        var result = _strategy.GetSimilarity("salam", "salam");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void GetSimilarityEmptyStringsReturnsZero()
    {
        Assert.Equal(1.0, _strategy.GetSimilarity("", ""));
        Assert.Equal(0.0, _strategy.GetSimilarity("abc", ""));
        Assert.Equal(0.0, _strategy.GetSimilarity("", "abc"));
    }

    [Fact]
    public void GetSimilarityDifferentStringsReturnsLessThanOne()
    {
        var result = _strategy.GetSimilarity("salam", "samal");
        Assert.InRange(result, 0.7, 1.0);  
    }

    [Fact]
    public void GetSimilarityNoCommonCharsReturnsZero()
    {
        var result = _strategy.GetSimilarity("abc", "xyz");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void GetSimilarityPrefixBoostApplied()
    {
        var result = _strategy.GetSimilarity("dixon", "dikson");
        Assert.InRange(result, 0.7, 0.9);
    }

    [Fact]
    public void GetSimilarityShortStrings()
    {
        var result = _strategy.GetSimilarity("a", "a");
        Assert.Equal(1.0, result);

        var result2 = _strategy.GetSimilarity("a", "b");
        Assert.Equal(0.0, result2);
    }
}