using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleGematria.Core.Dto
{
    // This matches the Sefaria JSON structure EXACTLY.
    // We only need the text and the title for now.
    public class SefariaBookDto
    {
        public string title { get; set; } // e.g., "Genesis"
        public string heTitle { get; set; } // e.g., "בראשית"

        // The text is a List (Chapters) of Lists (Verses) of Strings (Text).
        public List<List<string>> text { get; set; }
    }
}
