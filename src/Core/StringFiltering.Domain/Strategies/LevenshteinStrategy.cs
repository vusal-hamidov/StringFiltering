using System.Buffers;
using StringFiltering.Domain.Interfaces;

namespace StringFiltering.Domain.Strategies;

public class LevenshteinStrategy : ITextFilterStrategy
{
    public double GetSimilarity(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1))
            return string.IsNullOrEmpty(s2) ? 1.0 : 0.0;

        if (string.IsNullOrEmpty(s2))
            return 0.0;

        // Работать выгоднее так, чтобы первая строка была короче
        if (s1.Length > s2.Length)
            (s1, s2) = (s2, s1);

        ReadOnlySpan<char> a = s1.AsSpan();
        ReadOnlySpan<char> b = s2.AsSpan();

        int n = a.Length;
        int m = b.Length;

        if (n == 0)
            return 0.0;

        if (a.SequenceEqual(b))
            return 1.0;

        ArrayPool<int> pool = ArrayPool<int>.Shared;

        int[] prev = pool.Rent(n + 1);
        int[] curr = pool.Rent(n + 1);

        try
        {
            // Инициализация
            for (int i = 0; i <= n; i++)
                prev[i] = i;

            for (int j = 1; j <= m; j++)
            {
                curr[0] = j;
                char bj = b[j - 1];

                // Минимально возможная дистанция в текущей строке
                int rowMin = curr[0];

                for (int i = 1; i <= n; i++)
                {
                    int cost = a[i - 1] == bj ? 0 : 1;

                    int deletion = prev[i] + 1;
                    int insertion = curr[i - 1] + 1;
                    int substitution = prev[i - 1] + cost;

                    int best = deletion;
                    if (insertion < best) 
                        best = insertion;
                    
                    if (substitution < best) 
                        best = substitution;

                    curr[i] = best;
                    if (best < rowMin) 
                        rowMin = best;
                }

                // Early cutoff: если минимальная дистанция в строке больше длины, смысла продолжать нет
                if (rowMin > n) 
                {
                    return 1.0 - (double)rowMin / m;
                }

                (prev, curr) = (curr, prev);
            }

            int dist = prev[n];
            return 1.0 - (double)dist / Math.Max(n, m);
        }
        finally
        {
            pool.Return(prev);
            pool.Return(curr);
        }
    }
}