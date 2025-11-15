using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Options;
using StringFiltering.Application.Interfaces;
using StringFiltering.Application.Options;
using StringFiltering.Domain.Interfaces;

namespace StringFiltering.Application.Services;

public class TextFilter(IEnumerable<ITextFilterStrategy> strategies, IOptions<FilteringOptions> filteringOptions) : ITextFilter
{
    private readonly double _threshold = filteringOptions.Value.Threshold;
    private readonly string[] _badWordsUpper = [.. filteringOptions.Value.BadWords.Select(x => x.ToUpperInvariant())];
    private readonly ITextFilterStrategy[] _strategiesArray = [.. strategies];
    private readonly ConcurrentDictionary<(string word, string badWord, ITextFilterStrategy strategy), double> _similarityCache = new();
    private readonly ConcurrentDictionary<string, bool> _wordResultCache = new();
    
    public string Filter(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var inputLength = input.Length;
        var result = new StringBuilder(inputLength);
        var i = 0;
        
        while (i < inputLength)
        {
            var ch = input[i]; 
            
            // if char is letter or digit - start of a word
            if (char.IsLetterOrDigit(ch))
            {
                var start = i;
                
                // find the end of the word (letters, digits, apostrophes, hyphens)
                while (i < inputLength)
                {
                    ch = input[i];
                    if (char.IsLetterOrDigit(ch) || ch == '\'' || ch == '-')
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                
                // get full word
                var word = input[start..i];
                var wordUpper = word.ToUpperInvariant();

                // check in result cache
                if (!_wordResultCache.TryGetValue(wordUpper, out var isBad))
                {
                    // if not in cache then check
                    isBad = IsBadWord(wordUpper);
                    _wordResultCache.TryAdd(wordUpper, isBad);
                }
                
                if (!isBad)
                {
                    result.Append(word);
                }
            }
            else
            {
                // this char is not part of a word 
                result.Append(ch);
                i++;
            }
        }

        return result.ToString();
    }

    private bool IsBadWord(string wordUpper)
    {
        // checking the word against all bad words
        foreach (var badWord in _badWordsUpper)
        {
            // check all strategies 
            foreach (var strategy in _strategiesArray)
            {
                var key = (wordUpper, badWord, strategy);
                var similarity = _similarityCache.GetOrAdd(key, static x => x.strategy.GetSimilarity(x.word, x.badWord));
                if (similarity >= _threshold)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
}