using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BibleGematria.Core.Dto;
using BibleGematria.Core.Models;

namespace BibleGematria.Core
{
    public static class TanachLoader
    {
        // Maqqaf (Hebrew Hyphen) and standard Hyphen
        private static readonly char[] SplitChars = new char[] { ' ', '\u05BE', '-' };

        public static List<Verse> LoadBook(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Json file not found", filePath);

            string jsonString = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            SefariaBookDto? rawBook = JsonSerializer.Deserialize<SefariaBookDto>(jsonString, options);

            if (rawBook == null || rawBook.text == null)
                return new List<Verse>();

            var verses = new List<Verse>();

            for (int i = 0; i < rawBook.text.Count; i++)
            {
                var chapter = rawBook.text[i];

                for (int j = 0; j < chapter.Count; j++)
                {
                    string verseText = NormalizeVerseText(chapter[j]);

                    var verseObj = new Verse
                    {
                        BookName = rawBook.title,
                        Chapter = i + 1,
                        VerseNumber = j + 1,
                        FullText = verseText
                    };

                    string[] rawWords = verseText.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

                    for (int k = 0; k < rawWords.Length; k++)
                    {
                        string word = rawWords[k];
                        string clean = GematriaCalculator.Normalize(word);

                        verseObj.Words.Add(new WordToken
                        {
                            Text = word,
                            CleanText = clean,
                            GematriaValue = GematriaCalculator.Compute(clean),
                            WordIndex = k,
                            HasEtnachta = word.Contains('֑')
                        });
                    }

                    verses.Add(verseObj);
                }
            }

            return verses;
        }

        private static string NormalizeVerseText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.Replace("(פ)", "")
                       .Replace("(ס)", "");

            var parts = text
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => t != "פ" && t != "ס" && t != "(פ)" && t != "(ס)");

            return string.Join(" ", parts).Trim();
        }
    }
}
