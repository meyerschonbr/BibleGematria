using Xunit;
using BibleGematria.Core;

namespace BibleGematria.Tests
{
    public class CalculatorTests
    {
        [Fact]
        public void Compute_SimpleName_ReturnsCorrectSum()
        {
            // Aleph (1) + Bet (2) = 3
            Assert.Equal(3, GematriaCalculator.Compute("אב"));
        }

        [Fact]
        public void Compute_WithVowels_IgnoresVowels()
        {
            // Bereshit: Bet(2)+Resh(200)+Aleph(1)+Shin(300)+Yod(10)+Tav(400) = 913
            string withVowels = "בְּרֵאשִׁית";
            Assert.Equal(913, GematriaCalculator.Compute(withVowels));
        }

        [Fact]
        public void Compute_FinalLetters_UsesStandardValue()
        {
            // Adam: Aleph(1) + Dalet(4) + Final Mem(40) = 45
            // Note: In Mispar Gadol, Final Mem would be 600, but we want Standard.
            Assert.Equal(45, GematriaCalculator.Compute("אדם"));
        }

        [Fact]
        public void Compute_WithMaqqaf_IgnoresMaqqaf()
        {
            // Ben-Yishai (Note the hyphen/Maqqaf in the middle)
            // Bet(2)+Nun(50) + Yod(10)+Shin(300)+Yod(10) = 52 + 320 = 372
            // The Maqqaf itself should be ignored.
            string text = "בֶּן־יִשָׁי";
            Assert.Equal(372, GematriaCalculator.Compute(text));
        }
        [Fact]
        public void LoadBook_SplitsMaqqafCorrectly()
        {
            // 1. Create a fake mini-JSON file for testing
            string json = @"{
        ""title"": ""TestBook"",
        ""text"": [
            [ ""בְּנֵי־יִשְׂרָאֵל"" ] 
        ]
    }";
            string tempPath = "test_book.json";
            System.IO.File.WriteAllText(tempPath, json);

            try
            {
                // 2. Load it
                var verses = TanachLoader.LoadBook(tempPath);

                // 3. Assertions
                var verse = verses[0];
                // Should have 2 words, not 1
                Assert.Equal(2, verse.Words.Count);
                Assert.Equal("בְּנֵי", verse.Words[0].Text);
                Assert.Equal("יִשְׂרָאֵל", verse.Words[1].Text);

                // Verify calculations
                Assert.Equal(62, verse.Words[0].GematriaValue); // Bnei (2+50+10) = 62
                Assert.Equal(541, verse.Words[1].GematriaValue); // Yisrael
            }
            finally
            {
                // Cleanup
                if (System.IO.File.Exists(tempPath)) System.IO.File.Delete(tempPath);
            }
        }
    }
}