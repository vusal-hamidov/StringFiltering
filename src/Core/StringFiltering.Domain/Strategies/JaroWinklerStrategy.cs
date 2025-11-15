using System.Buffers;
using StringFiltering.Domain.Interfaces;

namespace StringFiltering.Domain.Strategies;

public class JaroWinklerStrategy : ITextFilterStrategy
{
    public double GetSimilarity(string s1, string s2)
    {
        if (s1 == s2)
            return 1.0;
        
        if (s1.Length == 0 || s2.Length == 0)
            return 0.0;

        int len1 = s1.Length;
        int len2 = s2.Length;

        int matchDistance = Math.Max(len1, len2) / 2 - 1;
        if (matchDistance < 0) 
            matchDistance = 0;

        // Allocate match arrays
        bool[]? rented1 = null;
        bool[]? rented2 = null;

        Span<bool> s1Matches = len1 <= 128
            ? stackalloc bool[len1]
            : (rented1 = ArrayPool<bool>.Shared.Rent(len1)).AsSpan(0, len1);

        Span<bool> s2Matches = len2 <= 128
            ? stackalloc bool[len2]
            : (rented2 = ArrayPool<bool>.Shared.Rent(len2)).AsSpan(0, len2);

        s1Matches.Clear();
        s2Matches.Clear();

        int matches = 0;

        // Count matches
        for (int i = 0; i < len1; i++)
        {
            int start = Math.Max(0, i - matchDistance);
            int end = Math.Min(len2 - 1, i + matchDistance);

            for (int j = start; j <= end; j++)
            {
                if (!s2Matches[j] && s1[i] == s2[j])
                {
                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    matches++;
                    break;
                }
            }
        }

        if (matches == 0)
        {
            ReturnIfPooled(rented1);
            ReturnIfPooled(rented2);
            return 0.0;
        }

        // Count transpositions
        int k = 0;
        int transpositions = 0;

        for (int i = 0; i < len1; i++)
        {
            if (!s1Matches[i])
                continue;

            while (!s2Matches[k])
                k++;

            if (s1[i] != s2[k])
                transpositions++;

            k++;
        }

        double m = matches;

        double jaro = (m / len1 + m / len2 + (m - transpositions / 2.0) / m) / 3.0;

        // Jaro-Winkler prefix
        int prefix = 0;
        int maxPrefix = Math.Min(4, Math.Min(len1, len2));

        for (int i = 0; i < maxPrefix; i++)
        {
            if (s1[i] == s2[i])
                prefix++;
            else
                break;
        }

        double result = jaro + 0.1 * prefix * (1 - jaro);

        // Release pooled arrays
        ReturnIfPooled(rented1);
        ReturnIfPooled(rented2);

        return result;
    }

    private static void ReturnIfPooled(bool[]? rented)
    {
        if (rented != null)
            ArrayPool<bool>.Shared.Return(rented);
    }
}