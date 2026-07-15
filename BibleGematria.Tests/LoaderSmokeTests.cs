using System.IO;
using System.Runtime.CompilerServices;
using BibleGematria.Core;
using Xunit;

namespace BibleGematria.Tests
{
    public class LoaderSmokeTests
    {
        // Resolves Genesis.json from BibleGematria.Wpf/Data relative to this source file,
        // so the test keeps working regardless of where the repo is cloned/moved.
        private static string GetGenesisJsonPath([CallerFilePath] string sourceFilePath = "")
        {
            string testsProjectDir = Path.GetDirectoryName(sourceFilePath)!;
            string repoRoot = Path.GetFullPath(Path.Combine(testsProjectDir, ".."));
            return Path.Combine(repoRoot, "BibleGematria.Wpf", "Data", "Genesis.json");
        }

        [Fact]
        public void LoadBook_Genesis_ParsesWithoutError()
        {
            string path = GetGenesisJsonPath();
            Assert.True(File.Exists(path), $"Expected Genesis.json at: {path}");  // Fail fast if the file isn’t there

            var verses = TanachLoader.LoadBook(path);

            // Basic sanity checks
            Assert.NotEmpty(verses);
            Assert.All(verses, v => Assert.False(string.IsNullOrWhiteSpace(v.FullText)));
            Assert.All(verses, v => Assert.NotNull(v.Words));
        }
    }
}