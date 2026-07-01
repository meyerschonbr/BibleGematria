using System.Collections.Generic;
using System.Linq;

namespace BibleGematria.Core.Models
{
    public enum BibleSection
    {
        Torah,
        Prophets,
        Writings
    }

    public class BibleBook
    {
        public string Key { get; }
        public string HebrewName { get; }
        public string FileName { get; }
        public BibleSection Section { get; }

        public BibleBook(string key, string hebrewName, string fileName, BibleSection section)
        {
            Key = key;
            HebrewName = hebrewName;
            FileName = fileName;
            Section = section;
        }
    }

    public static class BibleBookCatalog
    {
        public static readonly IReadOnlyList<BibleBook> Books = new List<BibleBook>
        {
            new("Genesis", "בראשית", "Genesis.json", BibleSection.Torah),
            new("Exodus", "שמות", "Exodus.json", BibleSection.Torah),
            new("Leviticus", "ויקרא", "Leviticus.json", BibleSection.Torah),
            new("Numbers", "במדבר", "Numbers.json", BibleSection.Torah),
            new("Deuteronomy", "דברים", "Deuteronomy.json", BibleSection.Torah),

            new("Joshua", "יהושע", "Joshua.json", BibleSection.Prophets),
            new("Judges", "שופטים", "Judges.json", BibleSection.Prophets),
            new("ISamuel", "שמואל א", "ISamuel.json", BibleSection.Prophets),
            new("IISamuel", "שמואל ב", "IISamuel.json", BibleSection.Prophets),
            new("IKings", "מלכים א", "IKings.json", BibleSection.Prophets),
            new("IIKings", "מלכים ב", "IIKings.json", BibleSection.Prophets),
            new("Isaiah", "ישעיהו", "Isaiah.json", BibleSection.Prophets),
            new("Jeremiah", "ירמיהו", "Jeremiah.json", BibleSection.Prophets),
            new("Ezekiel", "יחזקאל", "Ezekiel.json", BibleSection.Prophets),
            new("Hosea", "הושע", "Hosea.json", BibleSection.Prophets),
            new("Joel", "יואל", "Joel.json", BibleSection.Prophets),
            new("Amos", "עמוס", "Amos.json", BibleSection.Prophets),
            new("Obadiah", "עובדיה", "Obadiah.json", BibleSection.Prophets),
            new("Jonah", "יונה", "Jonah.json", BibleSection.Prophets),
            new("Micah", "מיכה", "Micah.json", BibleSection.Prophets),
            new("Nahum", "נחום", "Nahum.json", BibleSection.Prophets),
            new("Habakkuk", "חבקוק", "Habakkuk.json", BibleSection.Prophets),
            new("Zephaniah", "צפניה", "Zephaniah.json", BibleSection.Prophets),
            new("Haggai", "חגי", "Haggai.json", BibleSection.Prophets),
            new("Zechariah", "זכריה", "Zechariah.json", BibleSection.Prophets),
            new("Malachi", "מלאכי", "Malachi.json", BibleSection.Prophets),

            new("Psalms", "תהילים", "Psalms.json", BibleSection.Writings),
            new("Proverbs", "משלי", "Proverbs.json", BibleSection.Writings),
            new("Job", "איוב", "Job.json", BibleSection.Writings),
            new("SongOfSongs", "שיר השירים", "SongOfSongs.json", BibleSection.Writings),
            new("Ruth", "רות", "Ruth.json", BibleSection.Writings),
            new("Lamentations", "איכה", "Lamentations.json", BibleSection.Writings),
            new("Ecclesiastes", "קהלת", "Ecclesiastes.json", BibleSection.Writings),
            new("Esther", "אסתר", "Esther.json", BibleSection.Writings),
            new("Daniel", "דניאל", "Daniel.json", BibleSection.Writings),
            new("Ezra", "עזרא", "Ezra.json", BibleSection.Writings),
            new("Nehemiah", "נחמיה", "Nehemiah.json", BibleSection.Writings),
            new("IChronicles", "דברי הימים א", "IChronicles.json", BibleSection.Writings),
            new("IIChronicles", "דברי הימים ב", "IIChronicles.json", BibleSection.Writings),
        };

        private static readonly Dictionary<string, BibleBook> ByKey =
            Books.ToDictionary(b => b.Key);

        public static BibleBook? GetByKey(string key) =>
            ByKey.TryGetValue(key, out var book) ? book : null;
    }
}
