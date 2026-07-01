using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleGematria.Core
{
    public class MatchResult
    {
        public string BookName { get; set; } = string.Empty;
        public int Chapter { get; set; }
        public int VerseNumber { get; set; }

        //Full verse text for displaying
        public string VerseText { get; set; } = string.Empty;

        //Contiguous phrase
        public string MatchedText {  get; set; } = string.Empty;
        public int StartWordIndex { get; set; }
        public int WordCount { get; set; }
      
    }
}
