using BibleGematria.Core;
using System.Collections.Generic;

namespace BibleGematria.Tests
{
    public class CsvExporterTests
    {
        // Builds a minimal MatchResult so we're not repeating ourselves across tests
        private static MatchResult MakeResult(string book, int chapter, int verse,
            string match, int wordCount, string verseText)
        {
            return new MatchResult
            {
                BookName = book,
                Chapter = chapter,
                VerseNumber = verse,
                MatchedText = match,
                WordCount = wordCount,
                VerseText = verseText
            };
        }

        [Fact]
        public void Export_HeaderBlockContainsSearchedTextAndGematriaValue()
        {
            var csv = CsvExporter.Export(new List<MatchResult>(), "אדם", 45);
            Assert.Contains("Searched Text,אדם", csv);
            Assert.Contains("Gematria Value,45", csv);
        }

        [Fact]
        public void Export_ColumnHeaderRowIsPresent()
        {
            var csv = CsvExporter.Export(new List<MatchResult>(), "אדם", 45);
            Assert.Contains("Book,Chapter,Verse,Match,Word Count,Full Verse", csv);
        }

        [Fact]
        public void Export_ResultRowContainsAllFields()
        {
            var result = MakeResult("Genesis", 1, 1, "אדם", 1, "בראשית ברא אלהים");
            var csv = CsvExporter.Export(new List<MatchResult> { result }, "אדם", 45);
            Assert.Contains("Genesis,1,1,אדם,1,בראשית ברא אלהים", csv);
        }

        [Fact]
        public void Export_FieldContainingCommaIsWrappedInQuotes()
        {
            // Verse text will often contain commas — must be quoted or it breaks column alignment
            var result = MakeResult("Genesis", 1, 1, "אדם", 1, "first part, second part");
            var csv = CsvExporter.Export(new List<MatchResult> { result }, "אדם", 45);
            Assert.Contains("\"first part, second part\"", csv);
        }

        [Fact]
        public void Export_FieldContainingQuoteIsEscaped()
        {
            var result = MakeResult("Genesis", 1, 1, "say \"hello\"", 1, "some verse");
            var csv = CsvExporter.Export(new List<MatchResult> { result }, "אדם", 45);
            Assert.Contains("\"say \"\"hello\"\"\"", csv);
        }

        [Fact]
        public void Export_EmptyResultsProducesHeaderBlockAndColumnRowOnly()
        {
            var csv = CsvExporter.Export(new List<MatchResult>(), "אדם", 45);
            var lines = csv.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
            // Should be exactly: "Searched Text,...", "Gematria Value,...", column header = 3 lines
            Assert.Equal(3, lines.Length);
        }
    }
}
