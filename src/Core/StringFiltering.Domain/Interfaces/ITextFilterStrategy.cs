namespace StringFiltering.Domain.Interfaces;

public interface ITextFilterStrategy
{
    double GetSimilarity(string s1, string s2);
}