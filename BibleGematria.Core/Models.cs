using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleGematria.Core.Models
{
    public class WordToken
    {
        public string Text { get; set; }
        public string CleanText {  get; set; }
        public int GematriaValue {  get; set; }
        public int WordIndex { get; set; }
        public bool HasEtnachta { get; set; }
    }
    public class Verse
    {
        public string BookName { get; set; }
        public int Chapter {  get; set; }
        public int VerseNumber { get; set; }
        public string FullText { get; set; }
        public List<WordToken> Words {  get; set; } = new List<WordToken>();
    
    }
}
