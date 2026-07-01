using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BibleGematria.Core.Models;

namespace BibleGematria.Core
{
    public class TanachRepository
    {
        private readonly string _dataDirectory;
        private readonly ConcurrentDictionary<string, List<Verse>> _cache = new();

        public TanachRepository(string dataDirectory)
        {
            _dataDirectory = dataDirectory;
        }

        public List<Verse> GetBook(BibleBook book) =>
            _cache.GetOrAdd(book.Key, _ =>
                TanachLoader.LoadBook(Path.Combine(_dataDirectory, book.FileName)));

        public List<Verse> GetBooks(IEnumerable<string> bookKeys)
        {
            var verses = new List<Verse>();
            foreach (var key in bookKeys)
            {
                var book = BibleBookCatalog.GetByKey(key);
                if (book != null)
                {
                    verses.AddRange(GetBook(book));
                }
            }

            return verses;
        }
    }
}
