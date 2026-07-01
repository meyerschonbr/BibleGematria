using System.Text;

namespace BibleGematria.Core
{
    public static class GematriaCalculator
    {

        private static readonly Dictionary<char, int> GematriaValues = new Dictionary<char, int>
        {
            {'א', 1}, {'ב', 2}, {'ג', 3}, {'ד', 4}, {'ה', 5},
            {'ו', 6}, {'ז', 7}, {'ח', 8}, {'ט', 9},
            {'י', 10},
            {'כ', 20}, {'ך', 20},
            {'ל', 30},
            {'מ', 40}, {'ם', 40},
            {'נ', 50}, {'ן', 50},
            {'ס', 60}, {'ע', 70},
            {'פ', 80}, {'ף', 80},
            {'צ', 90}, {'ץ', 90},
            {'ק', 100}, {'ר', 200}, {'ש', 300}, {'ת', 400}
        };

        public static int Compute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            // Normalize first so tests with niqqud/maqqaf still work.
            string clean = Normalize(input);

            int total = 0;

            foreach (char c in clean)
            {
                if (GematriaValues.TryGetValue(c, out int val))
                {
                    total += val;
                }
            }

            return total;
        }
        public static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var sb = new StringBuilder(input.Length);

            foreach (char c in input)
            {
                // Hebrew letters Alef (U+05D0) through Tav (U+05EA). [web:27]
                if (c >= '\u05D0' && c <= '\u05EA')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
