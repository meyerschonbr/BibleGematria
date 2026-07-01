using BibleGematria.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleGematria.Core
{
    public class SearchService
    {
        private readonly List<Verse> _verses;
        private readonly Dictionary<int, List<(Verse verse, WordToken word)>> _singleWordIndex;
        public int MaxPhraseLength { get; set; } = 15;

        public SearchService(List<Verse> verses)
        {
            _verses = verses;
            _singleWordIndex = BuildSingleWordIndex(verses);
        }

        private static Dictionary<int, List<(Verse verse, WordToken word)>>
        BuildSingleWordIndex(List<Verse> verses)
        {
            var index = new Dictionary<int, List<(Verse, WordToken)>>();
            foreach (var verse in verses)
            {
                foreach (var word in verse.Words)
                {
                    int value = word.GematriaValue;
                    if (value <= 0)
                        continue;
                    if(!index.TryGetValue(value, out var list))
                    {
                        list = new List<(Verse, WordToken)> ();
                        index[value] = list; 
                    }
                    list.Add((verse, word));
                }
            }
            return index;
        }
        public List<MatchResult> FindSingleWordMatches(int target)
        {
            var results = new List<MatchResult>();
            if(!_singleWordIndex.TryGetValue(target, out var hits))
                return results;

            foreach (var (verse, word) in hits)
            {
                results.Add(new MatchResult
                {
                    BookName = verse.BookName,
                    Chapter = verse.Chapter,
                    VerseNumber = verse.VerseNumber,
                    VerseText = verse.FullText,
                    MatchedText = word.Text,
                    StartWordIndex = word.WordIndex,
                    WordCount = 1
                });
            }
            return results;
        }
        public List<MatchResult> FindPhraseMatchesNoBoundary(int target)
        {
            var results = new List<MatchResult>();
            foreach (var verse in _verses)
            {
                if (verse.Words.Count == 0)
                    continue;

                int n = verse.Words.Count;
                var prefix = new int[n + 1];
                for (int i = 0; i < n; i++)
                    prefix[i + 1] = prefix[i] + verse.Words[i].GematriaValue;

                for (int start = 0; start < n; start++)
                {
                    int maxLen = System.Math.Min(MaxPhraseLength, n - start);
                    for (int len = 2; len <= maxLen; len++)
                    {
                        // skip if any word except the last carries an etnachta (phrase break within match)
                        bool crossesBoundary = false;
                        for (int k = start; k < start + len - 1; k++)
                        {
                            if (verse.Words[k].HasEtnachta) { crossesBoundary = true; break; }
                        }
                        if (crossesBoundary) continue;

                        int sum = prefix[start + len] - prefix[start];
                        if (sum == target)
                        {
                            string phrase = string.Join(" ", verse.Words
                                .Skip(start)
                                .Take(len)
                                .Select(w => w.Text));
                            results.Add(new MatchResult
                            {
                                BookName = verse.BookName,
                                Chapter = verse.Chapter,
                                VerseNumber = verse.VerseNumber,
                                VerseText = verse.FullText,
                                MatchedText = phrase,
                                StartWordIndex = start,
                                WordCount = len
                            });
                        }
                    }
                }
            }
            return results;
        }

        public List<MatchResult> FindPhraseMatches(int target)
        {
            var results = new List<MatchResult>();
            foreach (var verse in _verses)
            {
                if (verse.Words.Count == 0)
                    continue;

                int n = verse.Words.Count;
                var prefix = new int[n + 1];
                for (int i = 0; i < n; i++)
                {
                    prefix[i + 1] = prefix[i] + verse.Words[i].GematriaValue;
                }
                for(int start = 0; start < n; start++)
                {
                    int maxLen = System.Math.Min(MaxPhraseLength, n - start);
                    for (int len = 1; len <= maxLen; len++)
                    {
                        int endExclusive = start + len;
                        int sum = prefix[endExclusive] - prefix[start];

                        if(sum == target)
                        {
                            string phrase = string.Join(" ", verse.Words
                                .Skip(start)
                                .Take(len)
                                .Select(w => w.Text));
                            results.Add(new MatchResult
                            {
                                BookName = verse.BookName,
                                Chapter = verse.Chapter,
                                VerseNumber = verse.VerseNumber,
                                VerseText = verse.FullText,
                                MatchedText = phrase,
                                StartWordIndex = start,
                                WordCount = len
                            });
                        }
                    }
                }
            }
            return results;
        }
    }
}
