using StringFiltering.Domain.Strategies;

namespace StringFiltering.UnitTests.Strategies;

public class LevenshteinStrategyTests
{
    private readonly LevenshteinStrategy _strategy = new();

    [Fact]
    public void GetSimilarityExactMatchReturnsOne()
    {
        var result = _strategy.GetSimilarity("salam", "salam");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void GetSimilarityBothEmptyStringsReturnsOne()
    {
        var result = _strategy.GetSimilarity("", "");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void GetSimilarityOneEmptyStringReturnsZero()
    {
        Assert.Equal(0.0, _strategy.GetSimilarity("", "abc"));
        Assert.Equal(0.0, _strategy.GetSimilarity("abc", ""));
    }

    [Fact]
    public void GetSimilarityCompletelyDifferentStringsReturnsZeroOrNearZero()
    {
        var result = _strategy.GetSimilarity("abc", "xyz");
        Assert.InRange(result, 0.0, 0.1);
    }

    [Fact]
    public void GetSimilarityPartialMatchReturnsBetweenZeroAndOne()
    {
        var result = _strategy.GetSimilarity("kitten", "sitting");
        Assert.InRange(result, 0.5, 1.0);
    }

    [Fact]
    public void GetSimilarityShortStrings()
    {
        Assert.Equal(1.0, _strategy.GetSimilarity("a", "a"));
        Assert.Equal(0.0, _strategy.GetSimilarity("a", "b"));
    }

    [Fact]
    public void GetSimilaritySymmetricResults()
    {
        const string a = "flaw";
        const string b = "lawn";

        var res1 = _strategy.GetSimilarity(a, b);
        var res2 = _strategy.GetSimilarity(b, a);

        Assert.Equal(res1, res2);
    }
}