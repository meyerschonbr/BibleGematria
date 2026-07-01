using BibleGematria.Core;
using BibleGematria.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleGematria.Tests
{
    public class SearchServiceTests
    {
        [Fact]
        public void FindPhraseMatches_FindsSimpleTwoWordPhrase()
        {
            var verse = new Verse
            {
                BookName = "Test",
                Chapter = 1,
                VerseNumber = 1,
                FullText = "אב ג ד"
            };
            verse.Words.Add(new WordToken { Text = "אב", CleanText = "אב", GematriaValue = GematriaCalculator.Compute("אב"), WordIndex = 0 }); //3
            verse.Words.Add(new WordToken { Text = "ג", CleanText = "ג", GematriaValue = GematriaCalculator.Compute("ג"), WordIndex = 1 });   // 3
            verse.Words.Add(new WordToken { Text = "ד", CleanText = "ד", GematriaValue = GematriaCalculator.Compute("ד"), WordIndex = 2 });   // 4

            var service = new SearchService(new List<Verse> { verse })
            {
                MaxPhraseLength = 3
            };
            int target = 6;
            var matches = service.FindPhraseMatches(target);
            Assert.Equal("אב ג", matches[0].MatchedText);
            Assert.Equal(0, matches[0].StartWordIndex);
            Assert.Equal(2, matches[0].WordCount);
        }
    }
}
