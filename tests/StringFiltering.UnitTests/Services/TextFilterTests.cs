using Microsoft.Extensions.Options;
using Moq;
using StringFiltering.Application.Options;
using StringFiltering.Application.Services;
using StringFiltering.Domain.Interfaces;

namespace StringFiltering.UnitTests.Services;

public class TextFilterTests
{
    private readonly Mock<ITextFilterStrategy> _exactStrategyMock = new();
    private readonly Mock<ITextFilterStrategy> _fuzzyStrategyMock = new();
    private readonly FilteringOptions _options = new()
    {
        Threshold = 0.8,
        BadWords = ["yaramaz", "axmaq", "ağılsız", "dələduz"]
    };

    private TextFilter CreateFilter(IEnumerable<ITextFilterStrategy>? strategies = null)
    {
        var strategyList = strategies?.ToList() ??
        [
            _exactStrategyMock.Object,
            _fuzzyStrategyMock.Object
        ];

        var options = Options.Create(_options);
        return new TextFilter(strategyList, options);
    }

    [Fact]
    public void FilterEmptyInputReturnsEmpty()
    {
        // Arrange
        var filter = CreateFilter();

        // Act
        var result = filter.Filter("");

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FilterNullInputReturnsNull()
    {
        // Arrange
        var filter = CreateFilter();

        // Act
        var result = filter.Filter(null!);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void FilterNoBadWordsReturnsOriginalText()
    {
        // Arrange
        _exactStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);
        
        _fuzzyStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);

        var filter = CreateFilter();

        // Act
        var result = filter.Filter("Salam mənim dostum!");

        // Assert
        Assert.Equal("Salam mənim dostum!", result);
    }

    [Fact]
    public void FilterExactBadWordRemovesWord()
    {
        // Arrange
        _exactStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((word, badWord) => word == badWord ? 1.0 : 0.0);

        _fuzzyStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);

        var filter = CreateFilter();

        // Act
        var result = filter.Filter("Salam mənim axmaq dostum!");

        // Assert
        Assert.Equal("Salam mənim  dostum!", result);
    }

    [Fact]
    public void FilterFuzzyMatchAboveThresholdRemovesWord()
    {
        // Arrange
        _fuzzyStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);

        _fuzzyStrategyMock
            .Setup(s => s.GetSimilarity("YARAMZ", "YARAMAZ"))
            .Returns(0.85); // > 0.8 → слово считается плохим

        _exactStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);

        var filter = CreateFilter();

        // Act
        var result = filter.Filter("O dedi sən yaramz adamsan!");

        // Assert
        Assert.Equal("O dedi sən  adamsan!", result);
    }
    
    [Fact]
    public void FilterFuzzyMatchBelowThresholdKeepsWord()
    {
        // Arrange
        _fuzzyStrategyMock
            .Setup(s => s.GetSimilarity("ALILSIZ", "AĞILSIZ"))
            .Returns(0.75); // < 0.8

        _fuzzyStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);

        _exactStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);

        var filter = CreateFilter();

        // Act
        var result = filter.Filter("Cox alılsız addımdır");

        // Assert
        Assert.Equal("Cox alılsız addımdır", result);
    }

    [Fact]
    public void FilterMultipleBadWordsRemovesAll()
    {
        // Arrange
        _exactStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);

        _exactStrategyMock
            .Setup(s => s.GetSimilarity("DƏLƏDUZ", "DƏLƏDUZ"))
            .Returns(1.0);

        _exactStrategyMock
            .Setup(s => s.GetSimilarity("AXMAQ", "AXMAQ"))
            .Returns(1.0);

        var filter = CreateFilter();

        // Act
        var result = filter.Filter("Dələduz və axmaq zəmanədir!");

        // Assert
        Assert.Equal(" və  zəmanədir!", result);
    }

    [Fact]
    public void FilterPunctuationAndSpacesPreserved()
    {
        // Arrange
        _exactStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);
        
        _exactStrategyMock
            .Setup(s => s.GetSimilarity("DƏLƏDUZ", "DƏLƏDUZ"))
            .Returns(1.0);

        var filter = CreateFilter();

        // Act
        var result = filter.Filter("Dələduz! Nə?! Zarafat edirsən...");

        // Assert
        Assert.Equal("! Nə?! Zarafat edirsən...", result);
    }

    [Fact]
    public void FilterWordWithApostropheTreatedAsOneWord()
    {
        // Arrange
        _exactStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);
        
        _exactStrategyMock
            .Setup(s => s.GetSimilarity("DƏLƏDUZLAR'", "DƏLƏDUZ"))
            .Returns(0.9);

        var filter = CreateFilter();

        // Act
        var result = filter.Filter("Sənin kimi dələduzlar' edir hamısını");

        // Assert
        Assert.Equal("Sənin kimi  edir hamısını", result);
    }

    [Fact]
    public void FilterHyphenatedWordTreatedAsOneWord()
    {
        // Arrange
        _exactStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(0.0);
        
        _exactStrategyMock
            .Setup(s => s.GetSimilarity("BÖYÜK-DƏLƏDUZLAR", "DƏLƏDUZ"))
            .Returns(0.9);

        var filter = CreateFilter();

        // Act
        var result = filter.Filter("O dəstənin adı sadəcə böyük-dələduzlar adlanır!");

        // Assert
        Assert.Equal("O dəstənin adı sadəcə  adlanır!", result);
    }

    [Fact]
    public void FilterCachePreventsRecalculation()
    {
        // Arrange
        var callCount = 0;
        
        _exactStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((w, b) =>
            {
                callCount++;
                return string.Equals(w, b, StringComparison.OrdinalIgnoreCase) ? 1.0 : 0.0;
            });

        var filter = CreateFilter();

        // Act
        filter.Filter("axmaq axmaq axmaq");
        filter.Filter("axmaq axmaq axmaq");
        filter.Filter("axmaq axmaq axmaq");

        // Assert
        Assert.Equal(2, callCount); // 1 bad words × 2 strategy = 2
    }

    [Fact]
    public void FilterSimilarityCacheWorksAcrossCalls()
    {
        // Arrange
        var similarityCalls = new List<(string word, string badWord)>();
        
        _fuzzyStrategyMock
            .Setup(s => s.GetSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((w, b) =>
            {
                similarityCalls.Add((w, b));
                return w == "DƏLƏDZ" && b == "DƏLƏDUZ" ? 0.9 : 0.0;
            });

        var filter = CreateFilter();

        // Act
        filter.Filter("O dedi dələdz");
        filter.Filter("Qız dedi dələdz olmaz");

        // Assert
        var matches = similarityCalls.Where(x => x == ("DƏLƏDZ", "DƏLƏDUZ")).ToList();
        Assert.Single(matches);
    }
}