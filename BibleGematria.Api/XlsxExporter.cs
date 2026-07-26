using BibleGematria.Core;
using ClosedXML.Excel;

namespace BibleGematria.Api
{
    public static class XlsxExporter
    {
        public static byte[] Export(IEnumerable<MatchResult> results, string searchedText, int gematriaValue)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Results");

            sheet.Cell(1, 1).Value = "Searched Text";
            sheet.Cell(1, 2).Value = searchedText;
            sheet.Cell(2, 1).Value = "Gematria Value";
            sheet.Cell(2, 2).Value = gematriaValue;

            string[] headers = { "Book", "Chapter", "Verse", "Match", "Word Count", "Full Verse" };
            for (int col = 0; col < headers.Length; col++)
            {
                sheet.Cell(3, col + 1).Value = headers[col];
            }

            int row = 4;
            foreach (var r in results)
            {
                sheet.Cell(row, 1).Value = r.BookName;
                sheet.Cell(row, 2).Value = r.Chapter;
                sheet.Cell(row, 3).Value = r.VerseNumber;
                sheet.Cell(row, 4).Value = r.MatchedText;
                sheet.Cell(row, 5).Value = r.WordCount;
                sheet.Cell(row, 6).Value = r.VerseText;
                row++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
