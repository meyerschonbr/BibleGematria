using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BibleGematria.Core
{
    public static class CsvExporter
    {
        public static string Export(IEnumerable<MatchResult> results, string searchedText, int gematriaValue)
        {
            var builder = new StringBuilder();

            //Header row
            builder.AppendLine($"Searched Text,{Escape(searchedText)}");
            builder.AppendLine($"Gematria Value,{gematriaValue}");

            //Results
            builder.AppendLine("Book,Chapter,Verse,Match,Word Count,Full Verse");

            foreach (var r in results)
            {
                builder.AppendLine($"{Escape(r.BookName)},{r.Chapter},{r.VerseNumber},{Escape(r.MatchedText)},{r.WordCount},{Escape(r.VerseText)}");
            }
            return builder.ToString();
        }
        private static string Escape(string value)
        {
            //For Csv - If value contains comma, quote, or new line - wrap it in 
            //double quotes and double any existing quotes inside it.
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}